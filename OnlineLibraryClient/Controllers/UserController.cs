using Gateway.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using OnlineLibraryClient.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OnlineLibraryClient.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient client;
        private readonly string baseUrl = "https://localhost:7190/user";

        public UserController(HttpClient client)
        {
            this.client = client;
        }
        /*public class LoginModel
        {
            [Required(ErrorMessage = "Поле електронної пошти є обов'язковим.")]
            [EmailAddress(ErrorMessage = "Поле електронної пошти не є дійсною адресою електронної пошти.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Поле Паролю є обов'язковим.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }*/
        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel register)
        {
            if (ModelState.IsValid)
            {
                var url = baseUrl + "/register";

                var jsonBody = JsonConvert.SerializeObject(register);
                var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    TempData["RegisteredUserEmail"] = register.Email;
                    TempData["SuccessMessage"] = "Успішно зареєстровано" ;
                    return RedirectToAction("Login");//View("Login", new LoginModel { SuccessMessage = "Успішно зареєстровано" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неправильно введені дані");
                    return View(register);
                }
            }
            ModelState.AddModelError(string.Empty, "Неправильно введені дані");
            return View(register);
        }

        public async Task<IActionResult> Login()
        {
            string? email = TempData["RegisteredUserEmail"] as string;
            string? message = TempData["SuccessMessage"] as string;
            if (email != null)
            {   
                return View(new LoginModel { Email = email, SuccessMessage = message});
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var url = baseUrl + "/login";

                var jsonBody = JsonConvert.SerializeObject(login);
                var body = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, body);
                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    var tokenObject = JObject.Parse(token);
                    var jwtToken = tokenObject.GetValue("token").ToString();

                    HttpContext.Session.SetString("JWTToken", jwtToken);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Неправильно введені Email чи Пароль");
                    return View(login);
                }
            }
            ModelState.AddModelError(string.Empty, "Неправильно введені Email чи Пароль");
            return View(login);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("JWTToken");

            return RedirectToAction("Index", "Home");
            //asp-route-returnUrl="@Url.Action("Index", "Home", new { area = "" }
        }
    }
}
