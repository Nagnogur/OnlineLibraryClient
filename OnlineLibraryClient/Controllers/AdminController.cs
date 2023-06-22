using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml.Linq;
using static OnlineLibraryClient.Controllers.AdminController;

namespace OnlineLibraryClient.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7190";

        public AdminController(HttpClient client)
        {
            this.client = client;
        }
        public IActionResult Index()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                return RedirectToAction("Login", "User");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var roles = decodedToken.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Select(s => s.Value);

            if (roles == null || !roles.Contains("Admin"))
            {
                return Unauthorized();
            }

            return View();
        }

        public class ServiceModel
        {
            public int Service { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ScanService([FromBody] ServiceModel serviceModel)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                return RedirectToAction("Login", "User");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = baseUrl + "/management/scan?service=" + serviceModel.Service;

            var response = await client.PostAsync(url, null);
            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Action completed successfully" });
            }
            else
            {
                return BadRequest(new { message = "Something went wrong" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageBackgroundTask(BackgroundModel model)
        {
            string message1 = "";
            string message2 = "";

            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                return RedirectToAction("Login", "User");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = baseUrl + "/management";

            if (model == null) { return BadRequest(); }
            if (model.TaskStatus != null)
            {
                var toggleUrl = url + "/togglescan?enable=";
                switch(model.TaskStatus)
                {
                    case "on":
                        {
                            toggleUrl += "true";
                            break;
                        }
                    case "off":
                        {
                            toggleUrl += "false";
                            break;
                        }
                }
                var responseToggle = await client.PatchAsync(toggleUrl, null);
                if (responseToggle.IsSuccessStatusCode)
                {
                    message1 = "Успішно встановлено стан";
                }
                else
                {
                    message1 = "Стан не було встановлено";
                }
            }

            if (model.Minutes >= 1)
            {
                var timerUrl = url + "/settimer?minutes=" + model.Minutes;
                var responseTimer = await client.PostAsync(timerUrl, null);
                if (responseTimer.IsSuccessStatusCode)
                {
                    message2 = "Успішно встановлено таймер";
                }
                else
                {
                    message2 = "Таймер не було змінено";
                }
            }

            return Ok(new { message = message1 + "\n" + message2 });
        }

    }

    public enum ServiceEnum
    {
        [Display(Name = "Google Books API")]
        GoogleBooks = 0,
        [Display(Name = "RoyalRoad.com")]
        RoyalRoad = 1,
    }

    public class BackgroundModel
    {
        public string? TaskStatus { get; set; }
        public double? Minutes { get; set; }
    }
}
