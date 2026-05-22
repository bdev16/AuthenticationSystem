using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationSystem.Services
{
    public class TokenService : ITokenService
    {
        // Gera o token Jwt
        public JwtSecurityToken GenerateAccessToken(IEnumerable<Claim> claims, IConfiguration _config)
        {
            // Captura a Key de dentro do appsettings
            var key = _config.GetSection("JWT").GetValue<string>("SecretKey") ??
                        throw new InvalidOperationException("Invalid secret Key");

            // Transforma a Key em um conjunto de bytes
            var privateKey = Encoding.UTF8.GetBytes(key);

            // Gera a assinatura do token
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(privateKey),
                                        SecurityAlgorithms.HmacSha256Signature); 

            // Define as claims do usuario e assinatura na estrutura descritiva do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_config.GetSection("JWT")
                                                    .GetValue<double>("TokenValidityInMinutes")),
                
                Audience = _config.GetSection("JWT")
                                    .GetValue<string>("ValidAudience"),

                Issuer = _config.GetSection("JWT").GetValue<string>("ValidIssuer"),
                SigningCredentials = signingCredentials
            };
            
            // Class voltada para fazer a validação do token
            var tokenHandler = new JwtSecurityTokenHandler();
            // Cria o token Jwt
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return token;
        }

        // Gera o refresh token
        public string GenerateRefreshToken()
        {
            // Gera uma varia que pode armazenar uma sequencia de bytes aleatorios
            var secureRandomBytes = new byte[128];

            // Gera uma instancia de uma classe que gerar numeros aleatorios
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            // Preenche a variavel com bytes aleatorios
            randomNumberGenerator.GetBytes(secureRandomBytes);

            // Converte a sequencia de bytes para o formato base64
            var resfreshToken = Convert.ToBase64String(secureRandomBytes);
            return resfreshToken;
        }

        // Captura as claims de usuario
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config)
        {
            // Captura a chave secreta do appsettings
            var secretKey = _config["JWT:SecretKey"] ?? throw new InvalidOperationException("Invalid Key");

            // Define as configurações de validação dos parametros do token
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                                        Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false
            };
            // Gera a instancia da classe responsavel por fazer o gerenciamento do token
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters,
                                                         out SecurityToken securityToken);
            
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                                !jwtSecurityToken.Header.Alg.Equals(
                                    SecurityAlgorithms.HmacSha256,
                                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
    }
}