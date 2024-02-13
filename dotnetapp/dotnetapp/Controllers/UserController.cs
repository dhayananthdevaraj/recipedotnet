using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using dotnetapp.Models;
using dotnetapp.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens; // Add this using directive
using Microsoft.AspNetCore.Authorization;  // Add this using directive for [Authorize]

[Route("api/")]
[ApiController]
public class AuthController : ControllerBase
{

    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    public class LoginRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [HttpPost("auth/login")]
    public async Task<ActionResult> GetUserByUsernameAndPassword([FromBody] LoginRequestModel request)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

            if (user == null)
            {
                return Ok(new { message = "Invalid Credentials" });
            }

            var token = AuthService.GenerateToken(user.UserId); // Access static method directly

            var responseObj = new
            {
                username = $"{user.FirstName} {user.LastName}",
                role = user.Role,
                token = token,
                userId = user.UserId
            };

            return Ok(responseObj);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("auth/register")]
    public async Task<ActionResult> AddUser([FromBody] User newUser)
    {
        try
        {
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Success" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("users")]
    public async Task<ActionResult> GetAllUsers()
    {
        try
        {
                  
            var users = await _context.Users.Select(u => new { u.FirstName, u.LastName, u.Role, u.UserId,u.Email,u.MobileNumber }).ToListAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    //     private string GenerateToken(int userId)
    //     {

    //         var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(HardcodedJwtSecretKey));
    //         var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    //         var claims = new[]
    //         {
    //             new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
    //             // Add additional claims if needed
    //         };

    //         var token = new JwtSecurityToken(
    //             claims: claims,
    //             expires: DateTime.UtcNow.AddHours(2), // Token expiry time
    //             signingCredentials: credentials
    //         );

    //         return new JwtSecurityTokenHandler().WriteToken(token);
    //     }
}
public class LoginRequestModel
{
    public string Email { get; set; }
    public string Password { get; set; }
}
