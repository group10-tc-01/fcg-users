using FCG.Users.Application.UseCases.Users.Register;
using FCG.Users.Application.UseCases.Users.UpdatePassword;
using FCG.Users.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Users.WebApi.Controllers.v1
{
    public class LiveDeployController(IMediator mediator) : FcgUserBaseController(mediator)
    {

        [HttpGet]
        public async Task<IActionResult> LiveDeploy()
        {
            return Ok(ApiResponse<string>.SuccesResponse("Live Deploy is working"));
        }
    }
}
