using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Medicloud.BLL.Helpers;

namespace Medicloud.Controllers
{
	public class PaymentController : Controller
	{
		private readonly HttpClient _httpClient;

		public PaymentController(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		[HttpPost]
		public async Task<IActionResult> CallBack(
			[FromQuery] string client_rrn,
			[FromQuery] string psp_rrn,
			[FromQuery] string client_ip_addr)
		{
			var response = await _httpClient.PostAsync(new Uri($"https://psp.mps.az/check?psp_rrn={psp_rrn}"), null);

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var apiResponse = JsonConvert.DeserializeObject<PaymentApiResponse>(content);
				if(apiResponse != null)
				{
					if(apiResponse.Code == 0 && apiResponse.Status == "OK") {
						return RedirectToAction("SuccessPayment", "Pricing");
					} else {
						return RedirectToAction("FailedPayment", "Pricing");
					}
				}
				return Ok(content);
			}

			return StatusCode((int)response.StatusCode, response.ReasonPhrase);
		}
	}
}