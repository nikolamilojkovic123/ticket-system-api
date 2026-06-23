using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TicketSystem.Application.Features.Users.Comands.UpsertGoogleUser;
using TicketSystem.Application.Mediator;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(SimpleMediator simpleMediator, IConfiguration configuration) : ControllerBase
{
    private readonly SimpleMediator _mediator = simpleMediator ?? throw new ArgumentNullException(nameof(simpleMediator));
    private readonly IConfiguration _configuration = configuration;

    [HttpGet("google-login")]
    public IActionResult GoogleLogin()
    {
        string redirectUrl = $"{Request.Scheme}://{Request.Host}/api/auth/google-response";

        AuthenticationProperties properties = new()
        {
            RedirectUri = redirectUrl
        };

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
        AuthenticateResult result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return BadRequest("Google authentication failed");

        string? email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
        string? firstName = result.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
        string? lastName = result.Principal.FindFirst(ClaimTypes.Surname)?.Value;
        string? googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        string? pictureUrl = result.Principal.FindFirst("urn:google:picture")?.Value;

        if (string.IsNullOrEmpty(email))
            return BadRequest("Email not found");

        Guid userId = await _mediator.Send(new UpsertGoogleUserCommand
        {
            Email = email,
            FirstName = firstName ?? "Unknown",
            LastName = lastName ?? "Unknown",
            GoogleId = googleId ?? string.Empty,
            ProfilePictureUrl = pictureUrl
        });

        string? name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
            return BadRequest("Email not found");

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name ?? email)
        };

        string? adminEmail = _configuration["AdminSettings:AdminEmail"];
        if (!string.IsNullOrWhiteSpace(adminEmail) &&
            email.Equals(adminEmail, StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim("role", "Admin"));
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

        SigningCredentials creds = new(
            key,
            SecurityAlgorithms.HmacSha256
        );

        JwtSecurityToken token = new(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        string jwt = new JwtSecurityTokenHandler().WriteToken(token);

        string uiBaseUrl = _configuration["UI:BaseUrl"] ?? "http://localhost:4200";
        return Redirect($"{uiBaseUrl}/login-callback?token={jwt}");
    }
}