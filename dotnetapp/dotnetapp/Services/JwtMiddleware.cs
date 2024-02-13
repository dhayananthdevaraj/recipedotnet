using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using dotnetapp.Services;
using Newtonsoft.Json;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var authorizationHeader = context.Request.Headers["Authorization"];
        var token = authorizationHeader.ToString().Replace("Bearer ", string.Empty);
        if (context.Request.Path.StartsWithSegments("/api/auth/login") ||
                  context.Request.Path.StartsWithSegments("/api/auth/register"))
        {
            await _next(context);
            return;
        }
        if (!string.IsNullOrEmpty(token) && AuthService.ValidateJwt(token))
        {
            Console.WriteLine("Validated");
            await _next.Invoke(context);
        }
        else
        {
        context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var responseMessage = new
            {
                message = "Invalid or expired token"
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(responseMessage));
            return;
        }
    }
}
