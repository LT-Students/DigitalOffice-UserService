using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces.Education;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Models.Dto.Responses;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EducationController : ControllerBase
    {
        [HttpPost("create")]
        public OperationResultResponse<Guid> Create(
            [FromServices] ICreateEducationCommand command,
            [FromBody] CreateEducationRequest request)
        {
            return command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromServices] IEditEducationCommand command,
            [FromQuery] Guid userId,
            [FromQuery] Guid educationId,
            [FromBody] JsonPatchDocument<EditEducationRequest> request)
        {
            return command.Execute(userId, educationId, request);
        }

        [HttpDelete("remove")]
        public OperationResultResponse<bool> Remove(
            [FromServices] IRemoveEducationCommand command,
            [FromQuery] Guid userId,
            [FromQuery] Guid educationId)
        {
            return command.Execute(userId, educationId);
        }
    }
}
