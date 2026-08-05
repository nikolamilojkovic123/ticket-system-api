using Microsoft.AspNetCore.Mvc;
using TicketSystem.API.Common;
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
        using Stream fileStream = file.OpenReadStream();
        Result<UploadDocumentResponse> result = await _mediator.Send(new UploadDocumentCommand(fileStream, file.FileName));

        if (!result.IsSuccess)
            return BadRequest(result.Errors);

        return Ok(new { documentId = result.Data!.DocumentId });
    }

    [HttpPost("{documentId:guid}/ask")]
    public async Task<IActionResult> AskStored(Guid documentId, [FromBody] AskStoredRequest request)
    {
        Result<DocumentAudioResponseDto> result = await _mediator.Send(
            new AskStoredDocumentCommand(documentId, request.Question, request.Voice));

        return result.ActionResult;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        Result<ICollection<DocumentListItemDto>> result = await _mediator.Send(new GetDocumentsQuery());

        return result.ActionResult;
    }

    [HttpPost("ask")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AskDocument(
        [FromForm] IFormFile file,
        [FromForm] string question,
        [FromForm] string voice)
    {
        using Stream fileStream = file.OpenReadStream();
        AskDocumentCommand command = new(fileStream, file.FileName, question, voice);
        Result<DocumentAudioResponseDto> result = await _mediator.Send(command);

        return result.ActionResult;
    }
}

