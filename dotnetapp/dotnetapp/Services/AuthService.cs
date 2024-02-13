using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace dotnetapp.Services
{
    public class AuthService
    {
        private const string HardcodedJwtSecretKey = "eqwRZa4whssssstedxfcgvgfcdxrwazfs"; // Replace with your actual secret key

        public static string GenerateToken(int userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(HardcodedJwtSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Token expiry time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    public static bool ValidateJwt(string token)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    Console.WriteLine("Token in serv"+token);
    var validationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(HardcodedJwtSecretKey)),
        ValidateIssuer = false, // Set to true if you want to validate the issuer
        ValidateAudience = false, // Set to true if you want to validate the audience
        RequireExpirationTime = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // No clock skew for a basic check
    };

    try
    {
        tokenHandler.ValidateToken(token, validationParameters, out _);
        Console.WriteLine("try");
        return true;
    }
    catch (Exception)
    {
        Console.WriteLine("Incatch");
        // Token validation failed
        return false;
    }
}

    }
}
