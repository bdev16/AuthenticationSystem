using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "E-mail is required")]
        [EmailAddress(ErrorMessage = "E-mail is invalid")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Password is required")]
        public string Password {get; set; }
    }
}