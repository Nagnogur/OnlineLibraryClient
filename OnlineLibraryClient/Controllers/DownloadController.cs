/*using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Mime;

namespace OnlineLibraryClient.Controllers
{
    public class DownloadController : Controller
    {
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7190";

        public DownloadController(ILogger<HomeController> logger, HttpClient client)
        {
            this.client = client;
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> Download(int bookId)
        {
            var url = baseUrl + "/api/download";

            var response = await client.GetAsync(url + "/" + bookId);
            if (response.IsSuccessStatusCode)
            {
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition.FileName;

                var fileStream = await response.Content.ReadAsStreamAsync();

                // Trigger file download in the browser
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

                return File(fileStream, );

            }          
        }
    }
}
*/