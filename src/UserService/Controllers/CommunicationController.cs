using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace LT.DigitalOffice.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CommunicationController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CommunicationController(
            [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("create")]
        public OperationResultResponse<Guid> Create(
            [FromServices] ICreateCommunicationCommand command,
            [FromBody] CreateCommunicationRequest request)
        {
            return command.Execute(request);
        }

        [HttpPatch("edit")]
        public OperationResultResponse<bool> Edit(
            [FromServices] IEditCommunicationCommand command,
            [FromBody] JsonPatchDocument<EditCommunicationRequest> request,
            [FromQuery] Guid communicationId)
        {
            return command.Execute(communicationId, request);
        }

        [HttpDelete("remove")]
        public OperationResultResponse<bool> Remove(
            [FromServices] IRemoveCommunicationCommand command,
            [FromQuery] Guid communicationId)
        {
            return command.Execute(communicationId);
        }
    }
}
