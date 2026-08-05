using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using TicketSystem.Application.Features.Users.Comands.ForgotPassword;
using TicketSystem.Application.Features.Users.Comands.LoginUser;
using TicketSystem.Application.Features.Users.Comands.RegisterUser;
using TicketSystem.Application.Features.Users.Comands.ResetPassword;
using TicketSystem.Application.Features.Users.Comands.UpsertGoogleUser;
using TicketSystem.Application.Features.Users.Common;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;
using TicketSystem.Domain.UserManagment.Enums;
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

        Result<AuthResult> upsertResult = await _mediator.Send(new UpsertGoogleUserCommand
        {
            Email = email,
            FirstName = firstName ?? "Unknown",
            LastName = lastName ?? "Unknown",
            GoogleId = googleId ?? string.Empty,
            ProfilePictureUrl = pictureUrl
        });

        string uiBaseUrl = _configuration["UI:BaseUrl"] ?? "http://localhost:4200";

        if (!upsertResult.IsSuccess || upsertResult.Data is null)
        {
            return Redirect($"{uiBaseUrl}/login-error?reason=google-email-taken");
        }

        string jwt = GenerateJwt(upsertResult.Data);

        return Redirect($"{uiBaseUrl}/login-callback?token={jwt}");
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand request, CancellationToken cancellationToken)
    {
        Result<AuthResult> result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess || result.Data is null)
            return Ok(new AuthApiResponse(false, result.Errors.FirstOrDefault()?.Message, null));

        return Ok(new AuthApiResponse(true, null,
            new AuthTokenResponse(GenerateJwt(result.Data), result.Data.UserId, result.Data.Role)));
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand request, CancellationToken cancellationToken)
    {
        Result<AuthResult> result = await _mediator.Send(request, cancellationToken);

        if (!result.IsSuccess || result.Data is null)
            return Ok(new AuthApiResponse(false, result.Errors.FirstOrDefault()?.Message, null));

        return Ok(new AuthApiResponse(true, null,
            new AuthTokenResponse(GenerateJwt(result.Data), result.Data.UserId, result.Data.Role)));
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        Result<bool> result = await _mediator.Send(request, cancellationToken);
        return Ok(new AuthApiResponse(result.IsSuccess, result.Errors.FirstOrDefault()?.Message, null));
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        Result<bool> result = await _mediator.Send(request, cancellationToken);
        return Ok(new AuthApiResponse(result.IsSuccess, result.Errors.FirstOrDefault()?.Message, null));
    }

    private string GenerateJwt(AuthResult authResult)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.NameIdentifier, authResult.UserId.ToString()),
            new Claim(ClaimTypes.Email, authResult.Email),
            new Claim(ClaimTypes.Name, authResult.FullName),
            new Claim("role", authResult.Role.ToString())
        };

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

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed record AuthTokenResponse(string Token, Guid UserId, UserRole Role);

public sealed record AuthApiResponse(bool Success, string? ErrorMessage, AuthTokenResponse? Data);