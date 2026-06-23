using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Services.Email;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/email-test")]
public class EmailTestController(IEmailService emailService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("send")]
    public async Task<IActionResult> SendTestEmail(CancellationToken ct)
    {
        string? adminEmail = configuration["AdminSettings:AdminEmail"];

        if (string.IsNullOrWhiteSpace(adminEmail))
            return BadRequest("AdminSettings:AdminEmail is not configured");

        await emailService.SendTicketCreatedAsync(
            Guid.NewGuid(),
            "Test Ticket - Email Integration",
            adminEmail,
            ct);

        return Ok($"Test email sent to {adminEmail}");
    }
}
