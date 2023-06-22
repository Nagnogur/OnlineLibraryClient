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
using NuGet.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text;
using System.Reflection;

namespace OnlineLibraryClient.Controllers
{
    public class WritingsController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7190";

        public WritingsController(ILogger<HomeController> logger, HttpClient client)
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

            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var email = decodedToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            var id = decodedToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
            var roles = decodedToken.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Select(s => s.Value);

            //  http......../api/{60raf-fasd......}/books}
            var url = baseUrl + "/api/" + id + "/books";

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var res = JsonConvert.DeserializeObject<List<BookModel>>(await response.Content.ReadAsStringAsync());
                return View(res);
            }
            else
            {
                return View(new List<BookModel>());
            }
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Description")] BookModel book, IFormFile bookFile, IFormFile thumbnailFile)//, IFormFile cover, int[] genre, int[] tag)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (token == null)
                {
                    return RedirectToAction("Login", "User");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (bookFile == null)
                {
                    return View(book);
                }

                byte[]? coverProcessed = null;

                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        if (ms.Length < 2097152)
                        {
                            thumbnailFile.CopyTo(ms);
                            coverProcessed = ms.ToArray();
                        }
                    }
                }

                book.ThumbnailFile = coverProcessed;

                // send file
                var fileUrl = baseUrl + "/useractions/savefile";
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(bookFile.OpenReadStream()), "File", Guid.NewGuid() + "_" + bookFile.FileName);
                var fileSaveResponse = await client.PostAsync(fileUrl, content);
                if (fileSaveResponse.IsSuccessStatusCode)
                {
                    var fileLoc = await fileSaveResponse.Content.ReadAsStringAsync();
                    book.FileLocation = fileLoc;
                }
                else
                {
                    return View(book);
                }


                // send book
                var url = baseUrl + "/useractions/create";

                var jsonBody = JsonConvert.SerializeObject(book);
                var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //book.ErrorMessage = "Something went wrong";
                    return View(book);
                }
            }
            catch
            {
                return View(book);
            }


            /*var result = await bookService.AddBook(mappedBook, coverProcessed, genre, tag);

            if (result)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["Genres"] = await tagAndGenreService.GetAllGenres();
                ViewData["Tags"] = await tagAndGenreService.GetAllTags();

                book.ErrorMessage = "Something went wrong";
                return View(book);
            }*/
        }

        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (token == null)
            {
                return RedirectToAction("Login", "User");
            }
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = "https://localhost:7190/api/books/" + id;

            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                BookModel? res = JsonConvert.DeserializeObject<BookModel>(await response.Content.ReadAsStringAsync());
                if (res != null)
                {
                    return View(res);
                }
            }
            else
            {

            }

            return NotFound();
        }

        //[Bind("BookId,Title,Subtitle,Publisher,Description,Page")]
        [HttpPost]
        public async Task<IActionResult> Edit(BookModel book, IFormFile bookFile, IFormFile thumbnailFile)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (token == null)
                {
                    return RedirectToAction("Login", "User");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                if (bookFile != null)
                {
                    var fileUrl = baseUrl + "/useractions/savefile";
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(bookFile.OpenReadStream()), "File", Guid.NewGuid() + "_" + bookFile.FileName);
                    var fileSaveResponse = await client.PostAsync(fileUrl, content);
                    if (fileSaveResponse.IsSuccessStatusCode)
                    {
                        var fileLoc = await fileSaveResponse.Content.ReadAsStringAsync();
                        book.FileLocation = fileLoc;
                    }
                }

                byte[]? coverProcessed = null;

                if (thumbnailFile != null && thumbnailFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        if (ms.Length < 2097152)
                        {
                            thumbnailFile.CopyTo(ms);
                            coverProcessed = ms.ToArray();
                        }
                    }
                    book.ThumbnailFile = coverProcessed;
                }

                var url = baseUrl + "/useractions/edit";

                var jsonBody = JsonConvert.SerializeObject(book);
                var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PutAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    //book.ErrorMessage = "Something went wrong";
                    return View(book);
                }
            }
            catch
            {
                return View(book);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("JWTToken");
                if (token == null)
                {
                    return RedirectToAction("Login", "User");
                }
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var url = baseUrl + "/useractions/" + id + "/delete";

                var response = await client.DeleteAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        } 
    }
}