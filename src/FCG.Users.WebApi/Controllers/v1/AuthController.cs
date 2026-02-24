using FCG.Users.Application.UseCases.Authentication.GenerateRefreshToken;
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
            var response = await _mediator.Send(request, CancellationToken.None);

            return FromResult(response, data => Ok(ApiResponse<LoginResponse>.SuccesResponse(data)));
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<RefreshTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var response = await _mediator.Send(request, CancellationToken.None);

            return FromResult(response, data => Ok(ApiResponse<RefreshTokenResponse>.SuccesResponse(data)));
        }
    }
}
