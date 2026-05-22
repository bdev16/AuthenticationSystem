using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationSystem.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using AuthenticationSystem.Services;
using AuthenticationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Internal.Mappers;

namespace AuthenticationSystem.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        
        public AuthController(UserManager<ApplicationUser> userManager,
        IConfiguration configuration, SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService, RoleManager<IdentityRole<int>> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole<int>(roleName));

                if (roleResult.Succeeded)
                {
                    return Ok("adicionado com sucesso!");
                }
                else
                {
                    return BadRequest("ocorreu algum erro!");
                }
            }
            return BadRequest("A role informada já existe!");
        }

        [HttpPost]
        [Route("AddUserToTole")]
        public async Task<IActionResult> AddUserToRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, roleName);
                if(result.Succeeded)
                {
                    return Ok($"O usuario {user.Email} foi adicionado a role {roleName}!");
                }
                else
                {
                    return BadRequest($"Não foi possivel adicionar o usuario {user.Email} na role {roleName}!");
                }
            }
            return BadRequest("Não foi possivel encontrar o usuario!");
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var userResult = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (userResult is not null && await _userManager.CheckPasswordAsync(userResult, loginDTO.Password))
            {
                // Captura as roles de usuario
                var userRoles = await _userManager.GetRolesAsync(userResult);

                // Cria uma lista de claims baseadas nas informaçõe de usuario
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userResult.UserName!),
                    new Claim(ClaimTypes.Email, userResult.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                // Cria novas claims baseadas nas roles do usuario
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

                var refreshToken = _tokenService.GenerateRefreshToken();

                // Recupera o tempo de expiração do refreshToken no appsettings
                _ = int.TryParse(_configuration["JWT:RefreshTokenValidityInMinutes"],
                                            out int refreshTokenValidityInMinutes);

                userResult.RefreshToken = refreshToken;

                userResult.RefreshTokenExpiryTime = 
                                        DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

                await _userManager.UpdateAsync(userResult);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("User desconected!");
        }

        [HttpPost("RegisterUser")]
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

            var user = new ApplicationUser
            {
              UserName = registerModel.UserName,
              Email = registerModel.Email,
              SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("User registered");
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                return BadRequest("Invalid client request");
            }

            string? acessToken = tokenModel.AcessToken 
                                    ?? throw new ArgumentNullException(nameof(tokenModel));

            string? refreshToken = tokenModel.RefreshToken
                                    ?? throw new ArgumentNullException(nameof(tokenModel));
        
            var principal = _tokenService.GetPrincipalFromExpiredToken(acessToken!, _configuration);

            if (principal == null)
            {
                return BadRequest("Invalid access token/refresh token");
            }

            // Captura o nome do usuario a partir do claim
            string username = principal.Identity!.Name!;

            var user = await _userManager.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != refreshToken
                                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return BadRequest("Invalid access token/refresh token");
            }

            var newAccessToken = _tokenService.GenerateAccessToken(
                                                principal.Claims.ToList(), _configuration);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return new ObjectResult(new
            {
               acessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
               refreshToken = newRefreshToken 
            });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return BadRequest("Invalid user name");
            
            user.RefreshToken = null;

            await _userManager.UpdateAsync(user);

            return NoContent();
        }
    }
}