using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineLibraryClient.Models.UserModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Поле електронної пошти є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Поле електронної пошти не є дійсною адресою електронної пошти.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Паролю є обов'язковим.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }
        [TempData]
        public string? SuccessMessage { get; set; }
        /*[BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }
        public class InputModel
        {
            [Required(ErrorMessage = "Поле електронної пошти є обов'язковим.")]
            [EmailAddress(ErrorMessage = "Поле електронної пошти не є дійсною адресою електронної пошти.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Поле Паролю є обов'язковим.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Запам'ятати мене")]
            public bool RememberMe { get; set; }
        }*/
    }
}
