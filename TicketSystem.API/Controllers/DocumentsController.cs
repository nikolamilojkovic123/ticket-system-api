using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Features.Documents;
using TicketSystem.Application.Features.Documents.Ask;
using TicketSystem.Application.Features.Documents.GetDocuments;
using TicketSystem.Application.Features.Documents.Upload;
using TicketSystem.Application.Mediator;
using TicketSystem.Core;

namespace TicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController(SimpleMediator simpleMediator) : ControllerBase
{
    private readonly SimpleMediator _mediator = simpleMediator ?? throw new ArgumentNullException(nameof(simpleMediator));

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Fajl nije prosleđen ili je prazan.");

        using Stream fileStream = file.OpenReadStream();
        Result<Guid> result = await _mediator.Send(new UploadDocumentCommand(fileStream, file.FileName));

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(new { documentId = result.Data });
    }

    [HttpPost("{documentId:guid}/ask")]
    public async Task<IActionResult> AskStored(Guid documentId, [FromBody] AskStoredRequest request)
    {
        Result<DocumentAudioResponseDto> result = await _mediator.Send(
            new AskStoredDocumentCommand(documentId, request.Question, request.Language));

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Result<ICollection<DocumentListItemDto>> result = await _mediator.Send(new GetDocumentsQuery());
        return Ok(result.Data);
    }

    [HttpPost("ask")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AskDocument(
        [FromForm] IFormFile file,
        [FromForm] string question,
        [FromForm] string language)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Fajl nije prosleđen ili je prazan.");

        using Stream fileStream = file.OpenReadStream();
        AskDocumentCommand command = new(fileStream, file.FileName, question, language);
        Result<DocumentAudioResponseDto> result = await _mediator.Send(command);
        return Ok(result);
    }
}

