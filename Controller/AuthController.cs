using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationSystem.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IConfiguration _configuration;
        
        public AuthController(UserManager<IdentityUser<int>> userManager,
        IConfiguration configuration, SignInManager<IdentityUser<int>> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterUser(RegisterUserDTO registerModel)
        {
            var usernameExist = await _userManager.FindByNameAsync(registerModel.UserName);
            var emailExist = await _userManager.FindByEmailAsync(registerModel.Email);

            if (usernameExist != null)
            {
                return Conflict("There is already a user with this username");
            }

            if (emailExist != null)
            {
                return Conflict("There is already a user with this email");
            }

            var user = new IdentityUser<int>
            {
              UserName = registerModel.UserName,
              Email = registerModel.Email,
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered");
        }

    }
}