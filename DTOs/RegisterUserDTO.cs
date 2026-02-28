using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationSystem.DTOs
{
    public class RegisterUserDTO
    {
        [Required(ErrorMessage = "Enter a Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a Email")]
        [EmailAddress(ErrorMessage = "E-mail is invalid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter a password")]
        public string Password { get; set; } = string.Empty;
    }
}