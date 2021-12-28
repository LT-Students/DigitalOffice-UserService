using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class CommunicationController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreateCommunicationCommand command,
      [FromBody] CreateCommunicationRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditCommunicationCommand command,
      [FromBody] JsonPatchDocument<EditCommunicationRequest> request,
      [FromQuery] Guid communicationId)
    {
      return await command.ExecuteAsync(communicationId, request);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveCommunicationCommand command,
      [FromQuery] Guid communicationId)
    {
      return await command.ExecuteAsync(communicationId);
    }
  }
}
