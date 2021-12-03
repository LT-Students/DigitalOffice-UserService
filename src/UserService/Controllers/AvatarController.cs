using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatar;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class AvatarController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreateAvatarCommand command,
      [FromBody] CreateAvatarRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<ImagesResponse>> GetAsync(
      [FromServices] IGetAvatarsCommand command,
      [FromQuery] Guid userId)
    {
      return await command.ExecuteAsync(userId);
    }

    [HttpDelete("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveAvatarsCommand command,
      [FromBody] RemoveAvatarsRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("editcurrent")]
    public async Task<OperationResultResponse<bool>> EditCurrentAsync(
      [FromServices] IEditAvatarCommand command,
      [FromQuery] Guid userId,
      [FromQuery] Guid avatarId)
    {
      return await command.ExecuteAsync(userId, avatarId);
    }
  }
}
