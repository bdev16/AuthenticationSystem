using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationSystem.DTO
{
    public class UserDTO
    {
        public int UserId { get; set; }
        
        [Required]
        public string UserName { get; set; } = string.Empty;
       
        [Required]
        [EmailAddress(ErrorMessage = "E-mail is invalid")]
        public string Email { get; set; } = string.Empty;
        
    }
}