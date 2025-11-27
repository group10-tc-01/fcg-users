using FCG.Users.Application.UseCases.Authentication.Login;
using FCG.Users.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.WebApi.Controllers.v1
{
    public class AuthController : FcgUserBaseController
    {
        public AuthController(IMediator mediator) : base(mediator) { }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _mediator.Send(request, CancellationToken.None).ConfigureAwait(false);

            return Ok(ApiResponse<LoginResponse>.SuccesResponse(response));
        }
    }
}
