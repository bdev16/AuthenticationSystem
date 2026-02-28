using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Enter a Email")]
        [EmailAddress(ErrorMessage = "E-mail is invalid")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Enter a Email")]
        public string Password { get; set; } = string.Empty;
    }
}