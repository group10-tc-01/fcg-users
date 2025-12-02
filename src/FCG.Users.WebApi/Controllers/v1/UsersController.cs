using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.Application.UseCases.Users.UpdatePassword;
using FCG.Users.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.WebApi.Controllers.v1
{
    public class UsersController(IMediator mediator) : FcgUserBaseController(mediator)
    {

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RegisterUserResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);

            return Created(string.Empty, ApiResponse<RegisterUserResponse>.SuccesResponse(response));
        }

        [HttpPatch("update-password")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UpdatePasswordResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(ApiResponse<UpdatePasswordResponse>.SuccesResponse(response));
        }
    }
}
