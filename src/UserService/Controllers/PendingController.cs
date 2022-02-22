using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.PendingUser.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class PendingController : ControllerBase
  {
    [HttpGet("check")]
    public async Task<OperationResultResponse<bool>> СheckAsync(
      [FromServices] ICheckPendingUserCommand command,
      [FromQuery] Guid userId)
    {
      return await command.ExecuteAsync(userId);
    }

    [HttpGet("find")]
    public async Task<FindResultResponse<UserInfo>> FindAsync(
      [FromServices] IFindPendingUserCommand command,
      [FromQuery] FindPendingUserFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }
  }
}
