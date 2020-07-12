using System.ComponentModel.DataAnnotations;

namespace WebTelegramBotsBuilder.Models.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
