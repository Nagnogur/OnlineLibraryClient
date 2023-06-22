using Gateway.Extensions;
using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OnlineLibraryClient.Models;
using ProcessingService.RetrieveLogic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;
using System.Text;

namespace OnlineLibraryClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7190";

        public HomeController(ILogger<HomeController> logger, HttpClient client)
        {
            _logger = logger;
            this.client = client;
        }

        public async Task<IActionResult> Index(BookQueryParameters parameters)
        {
            try
            {
                var res = await Filter(parameters);
                return View(parameters);
            }
            catch (Exception )
            {
                ViewData["Books"] = new List<BookModel>();
                return View();
            }   
        }

        [Route("{id}/links")]
        public async Task<IActionResult> Links(int id)
        {
            var user = User.Identity;
            var token = HttpContext.Session.GetString("JWTToken");
            if (token != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var url = baseUrl + "/api/books";

            var response = await client.GetAsync(url + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<BookModel>(await response.Content.ReadAsStringAsync());
                return View(res);
            }
            else
            {
                return View(null);
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Filter(BookQueryParameters parameters)
        {
            var url = baseUrl + "/api/books";
            //var fullUrl = sourceUrl + Request.QueryString.Value;

            string queryString = parameters.GetQueryString();

            var response = await client.GetAsync(url + "?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<List<BookModel>>(await response.Content.ReadAsStringAsync());
                ViewData["Books"] = res;
            }
            else
            {
                return View(null);
            }

            return View();
        }

        // GET api/<GatewayController>
        [HttpGet]
        public async Task<IActionResult> Post([FromBody] BookQueryParameters query)
        {
            var url = baseUrl + "/api/books";   
            //var fullUrl = sourceUrl + Request.QueryString.Value;

            string queryString = query.GetQueryString();

            var response = await client.GetAsync(url + "?" + queryString);
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<List<BookModel>>(await response.Content.ReadAsStringAsync());
                return View(res);
            }
            else
            {
                return View(null);
            }

            return View();
        }

        [HttpPost]
        [Route("rate")]
        public async Task<IActionResult> Rate([FromBody] RatingRequestModel request)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                return Unauthorized();//RedirectToAction("Login", "User");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid request data");
            }

            var url = baseUrl + "/useractions/rate";

            var jsonBody = JsonConvert.SerializeObject(request);
            var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, body);
            if (response.IsSuccessStatusCode)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }
        public class RatingRequestModel
        {
            public int BookId { get; set; }
            public int Rating { get; set; }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /*public async Task DownloadBook(int bookId)
        {
            using (var httpClient = new HttpClient())
            {
                var url = baseUrl + "/api/download";

                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var contentDisposition = response.Content.Headers.ContentDisposition;
                    var fileName = contentDisposition.FileName;

                    var fileStream = await response.Content.ReadAsStreamAsync();

                    // Convert the file stream to a byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        await fileStream.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();

                        // Set the download URL and trigger the download
                        var downloadElement = document.GetElementById("download-link");
                        downloadElement.SetAttribute("href", $"data:application/octet-stream;base64,{Convert.ToBase64String(fileBytes)}");
                        downloadElement.SetAttribute("download", fileName);

                        // Programmatically trigger the click event to start the download
                        downloadElement.Click();
                    }
                }
                else
                {
                    // Handle error case
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Download failed: {errorMessage}");
                }
            }
        }*/
    }
}