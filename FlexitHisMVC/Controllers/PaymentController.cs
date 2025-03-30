using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using Medicloud.BLL.Models;
using Medicloud.BLL.Utils;
using Medicloud.DAL.Repository;
using Medicloud.Models;
using Microsoft.AspNetCore.Authorization;
using Medicloud.BLL.Services;
using Medicloud.DAL.Repository.Plan;

namespace Medicloud.Controllers
{
    public class PaymentController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string ConnectionString;
        public IConfiguration Configuration;

        private readonly PaymentRepo paymentRepo;
        private readonly IUserService _userService;

        private readonly int _serviceId;
        private readonly string _clientRrn;
        private readonly string _clientIp;
        private readonly string _secretKey;
        private string _hash;

        public PaymentController(IConfiguration configuration, HttpClient httpClient,IUserService userService,IPlanRepository planRepository)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _httpClient = httpClient;
            _serviceId = 1394;
            _clientRrn = Guid.NewGuid().ToString();
            _clientIp = "188.213.212.170";
            _secretKey = "Medicloud1234";
            paymentRepo = new PaymentRepo(ConnectionString,planRepository);
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> callback(
            [FromQuery] string client_rrn,
            [FromQuery] string psp_rrn,
             [FromQuery] int month,
            [FromQuery] int planId,
            [FromQuery] int userID,
            [FromQuery] string client_ip_addr
           )
        {

            PaymentViewModel pvm = new()
            {
                client_ip_addr = client_ip_addr,
                client_rrn = client_rrn,
                psp_rrn = psp_rrn,
                user_id = userID,
                status = -1,
                payment_reason_id = 1,
                month = Convert.ToInt32(month),
                plan_id = Convert.ToInt32(planId)
            };
            try
            {
                var response = await _httpClient
                    .PostAsync(new Uri($"https://psp.mps.az/check?psp_rrn={psp_rrn}"), null);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<PaymentApiResponse>(content);


                    if (apiResponse != null && apiResponse.Status == "OK")
                    {
                        pvm.status = apiResponse.Code;
                    }
                }

                //pvm.status = 0;

                //if(pvm.status == 0)
                //{
                //	var user = userRepo.GetUserByID(pvm.user_id);
                //	if (user.subscription_expire_date.HasValue && user.subscription_expire_date.Value >= DateTime.Now)
                //	{
                //		pvm.expireDate = user.subscription_expire_date.Value.AddMonths(month);
                //	}
                //	else
                //	{
                //		pvm.expireDate = DateTime.Now.AddMonths(month);
                //	}
                //}

                paymentRepo.AddTransaction(pvm);

                if (pvm.status == 0)
                {
                    return RedirectToAction("SuccessPayment", "Pricing");
                }
                else if (pvm.status == 1)
                {
                    return RedirectToAction("PendingPayment", "Pricing");
                }
                else
                {
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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Process([FromQuery] int amount, int month, int planId)
        {
            var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");

            _hash = HMACUtils.ComputeHMACSHA256($"{_serviceId}{_clientRrn}{amount}", _secretKey);
            string url = $"https://psp.mps.az/process?service_id={_serviceId}&client_rrn={_clientRrn}&amount={amount}&client_ip={_clientIp}&hash={_hash}&month={month}&planId={planId}&userID={userID}";

            return Ok(url);
        }
    }
}