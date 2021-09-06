using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Avatar;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class AvatarController : ControllerBase
  {
    [HttpPost("add")]
    public OperationResultResponse<List<Guid>> Add(
      [FromServices] IAddAvatarsCommand command,
      [FromBody] AddAvatarRequest request)
    {
      return command.Execute(request);
    }

    [HttpGet("get")]
    public OperationResultResponse<AvatarsResponse> Get(
      [FromServices] IGetAvatarsCommand command,
      [FromQuery] Guid userId,
      [FromQuery] bool getOnlyCurrent = false)
    {
      return command.Execute(userId, getOnlyCurrent);
    }

    [HttpPost("remove")]
    public OperationResultResponse<bool> Remove(
      [FromServices] IRemoveAvatarsCommand command,
      [FromBody] RemoveAvatarsRequest request)
    {
      return command.Execute(request);
    }
  }
}
