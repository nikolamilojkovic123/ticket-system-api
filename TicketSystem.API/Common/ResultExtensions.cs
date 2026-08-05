using Microsoft.AspNetCore.Mvc;
using TicketSystem.Application.Common;
using TicketSystem.Core;

namespace TicketSystem.API.Common;

public static class ResultExtensions
{
    extension(Result result)
    {
        public IActionResult ActionResult
        {
            get
            {
                if (result.IsSuccess)
                {
                    return new OkResult();
                }

                if (result.Errors.Exists(x => x.Equals(ApplicationErrors.NotFound)))
                {
                    return new NotFoundObjectResult(result.Errors);
                }
                if (result.Errors.Exists(x => x.Equals(ApplicationErrors.Unauthorized)))
                {
                    return new UnauthorizedObjectResult(result.Errors);
                }
                if (result.Errors.Exists(x => x.Equals(ApplicationErrors.Forbidden)))
                {
                    return new ObjectResult(result.Errors) { StatusCode = StatusCodes.Status403Forbidden };
                }

                return new BadRequestObjectResult(result.Errors);
            }
        }
    }

    extension<T>(Result<T> result) where T : class
    {
        public IActionResult ActionResult
        {
            get
            {
                if (result.IsSuccess)
                {
                    return new OkObjectResult(result.Data);
                }

                if (result.Errors.Exists(x => x.Equals(ApplicationErrors.NotFound)))
                {
                    return new NotFoundObjectResult(result.Errors);
                }
                if (result.Errors.Exists(x => x.Equals(ApplicationErrors.Unauthorized)))
                {
                    return new UnauthorizedObjectResult(result.Errors);
                }
                if (result.Errors.Exists(x => x.Equals(ApplicationErrors.Forbidden)))
                {
                    return new ObjectResult(result.Errors) { StatusCode = StatusCodes.Status403Forbidden };
                }

                return new BadRequestObjectResult(result.Errors);
            }
        }
    }
}
