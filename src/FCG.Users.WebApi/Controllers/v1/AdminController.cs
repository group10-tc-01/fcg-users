using FCG.Users.Application.Abstractions.Pagination;
using FCG.Users.Application.UseCases.Admin.CreateUser;
using FCG.Users.Application.UseCases.Admin.DeactivateUser;
using FCG.Users.Application.UseCases.Admin.GetUsers;
using FCG.Users.Application.UseCases.Admin.UpdateUserRole;
using FCG.Users.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.WebApi.Controllers.v1
{
    [Route("api/v1/admin/users")]
    [ApiController]
    public class AdminController : FcgUserBaseController
    {
        public AdminController(IMediator mediator) : base(mediator) { }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedListResponse<GetUsersResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersRequest request)
        {
            var response = await _mediator.Send(request);

            return Ok(ApiResponse<PagedListResponse<GetUsersResponse>>.SuccesResponse(response));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreateUserResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var response = await _mediator.Send(request);

            return CreatedAtAction(nameof(CreateUser), ApiResponse<CreateUserResponse>.SuccesResponse(response));
        }

        [HttpPatch("{id}/update-role")]
        [ProducesResponseType(typeof(ApiResponse<UpdateUserRoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole([FromRoute] Guid id, [FromBody] UpdateUserRoleBodyRequest bodyRequest, CancellationToken cancellationToken)
        {
            var request = new UpdateUserRoleRequest(id, bodyRequest.NewRole);
            var response = await _mediator.Send(request, cancellationToken);

            return Ok(ApiResponse<UpdateUserRoleResponse>.SuccesResponse(response));
        }

        [HttpPatch("{id}/deactivate")]
        [ProducesResponseType(typeof(ApiResponse<DeactivateUserResponse>),  StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new DeactivateUserRequest(id);
            var response = await _mediator.Send(request, cancellationToken);
            return Ok(ApiResponse<DeactivateUserResponse>.SuccesResponse(response));
        }
    }
}
