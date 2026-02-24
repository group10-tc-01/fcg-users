using Asp.Versioning;
using FCG.Users.Application.Abstractions.Results;
using FCG.Users.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.WebApi.Controllers.v1
{
    [ExcludeFromCodeCoverage]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FcgUserBaseController(IMediator mediator) : ControllerBase
    {
        protected IMediator _mediator = mediator;

        protected IActionResult FromResult<T>(Result<T> result, Func<T, IActionResult> onSuccess)
        {
            if (result.IsSuccess && result.Value is not null)
            {
                return onSuccess(result.Value);
            }

            return StatusCode((int)result.StatusCode, ApiResponse<T>.ErrorResponse(new List<string> { result.ErrorMessage! }, result.StatusCode));
        }
    }
}
