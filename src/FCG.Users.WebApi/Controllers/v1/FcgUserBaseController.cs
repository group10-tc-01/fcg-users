using Asp.Versioning;
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
    }
}
