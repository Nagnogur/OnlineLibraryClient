using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static OnlineLibraryClient.Controllers.HomeController;
using System.Net.Http.Headers;
using System.Text;
using Gateway.Models;

namespace OnlineLibraryClient.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7190/useractions/getrecommended";

        public RecommendationController(ILogger<HomeController> logger, HttpClient client)
        {
            _logger = logger;
            this.client = client;
        }
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                return RedirectToAction("Login", "User");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var books = await GetRecommendations();
            if (books == null)
            {
                books = new List<BookModel>();
                // viewdata
            }
            return View(books);
        }

        [HttpGet]
        public async Task<List<BookModel>?> GetRecommendations()
        {            
            var url = baseUrl;

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<List<BookModel>>(await response.Content.ReadAsStringAsync());
                return res;
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    RedirectToAction("Login", "User");
                }
                return null;
            }
        }
    }
}
