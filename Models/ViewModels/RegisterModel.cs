using System.ComponentModel.DataAnnotations;

namespace WebTelegramBotsBuilder.Models.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(maximumLength:12,MinimumLength = 4, ErrorMessage = "Name will have 4-12 symbols")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(maximumLength: 20, MinimumLength = 8, ErrorMessage = "Password will have 8-20 symbols")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage = "Passwords is different")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
