using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using Medicloud.BLL.Models;
using Medicloud.BLL.Utils;
using Medicloud.DAL.Repository;

namespace Medicloud.Controllers
{
	public class PaymentController : Controller
	{
		private readonly HttpClient _httpClient;
		private readonly string ConnectionString;
		public IConfiguration Configuration;

		private readonly PaymentRepo paymentRepo;

		private readonly int _serviceId;
		private readonly string _clientRrn;
		private readonly string _clientIp;
		private readonly string _secretKey;
		private string _hash;

		public PaymentController(IConfiguration configuration, HttpClient httpClient)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_httpClient = httpClient;
			_serviceId = 1394;
			_clientRrn = Guid.NewGuid().ToString();
			_clientIp = "188.213.212.170";
			_secretKey = "Medicloud1234";
			paymentRepo = new PaymentRepo(ConnectionString);
		}

		[HttpPost]
		public async Task<IActionResult> CallBack(
			[FromQuery] string client_rrn,
			[FromQuery] string psp_rrn,
			[FromQuery] string client_ip_addr)
		{
			PaymentViewModel pvm = new()
			{
				client_ip_addr = client_ip_addr,
				client_rrn = client_rrn,
				psp_rrn = psp_rrn,
				user_id = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")),
				status = -1,
				payment_reason_id = 1
			};
			try
			{
				var response = await _httpClient.PostAsync(new Uri($"https://psp.mps.az/check?psp_rrn={psp_rrn}"), null);

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var apiResponse = JsonConvert.DeserializeObject<PaymentApiResponse>(content);


					if (apiResponse != null && apiResponse.Status == "OK")
					{
						pvm.status = apiResponse.Code;
					}
				}

				paymentRepo.AddTransaction(pvm);

				if (pvm.status == 0) {
					return RedirectToAction("SuccessPayment", "Pricing");
				} else if (pvm.status == 1) {
					return RedirectToAction("Pending", "Pricing");
				} else {
					return RedirectToAction("FailedPayment", "Pricing");
				}

			}
			catch (Exception ex)
			{
				pvm.status = -1;
				paymentRepo.AddTransaction(pvm);
				return RedirectToAction("FailedPayment", "Pricing");
			}
		}


		[HttpPost]
		public async Task<IActionResult> Process([FromQuery] int amount)
		{
			_hash = HMACUtils.ComputeHMACSHA256($"{_serviceId}{_clientRrn}{amount}", _secretKey);
			string url = $"https://psp.mps.az/process?service_id={_serviceId}&client_rrn={_clientRrn}&amount={amount}&client_ip={_clientIp}&hash={_hash}";

			return Ok(url);
		}
	}
}