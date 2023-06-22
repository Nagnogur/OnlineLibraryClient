using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace OnlineLibraryClient.Models.UserModel
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Поле електронної пошти є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Поле електронної пошти не є дійсною адресою електронної пошти.")]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Паролю є обов'язковим.")]
        [StringLength(100, ErrorMessage = "{0} має бути не менше {2} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Поле {0} є обов'язковим.")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження паролю")]
        [Compare("Password", ErrorMessage = "Не збігається з полем Паролю.")]
        public string ConfirmPassword { get; set; }

        public string? ErrorMessage { get; set; }
    }
}