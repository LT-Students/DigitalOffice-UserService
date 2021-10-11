using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class EducationController : ControllerBase
  {
    [HttpPost("create")]
    public OperationResultResponse<Guid?> Create(
      [FromServices] ICreateEducationCommand command,
      [FromBody] CreateEducationRequest request)
    {
      return command.ExecuteAsync(request);
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> Edit(
      [FromServices] IEditEducationCommand command,
      [FromQuery] Guid educationId,
      [FromBody] JsonPatchDocument<EditEducationRequest> request)
    {
      return await command.ExecuteAsync(educationId, request);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> Remove(
      [FromServices] IRemoveEducationCommand command,
      [FromQuery] Guid educationId)
    {
      return await command.ExecuteAsync(educationId);
    }
  }
}
