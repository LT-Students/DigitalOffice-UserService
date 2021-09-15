using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ImageController : ControllerBase
  {
    [HttpPost("add")]
    public OperationResultResponse<List<Guid>> Add(
      [FromServices] IAddImagesCommand command,
      [FromBody] AddImagesRequest request)
    {
      return command.Execute(request);
    }

    [HttpPost("updateAvatar")]
    public OperationResultResponse<Guid?> UpdateAvatar(
      [FromServices] IUpdateAvatarCommand command,
      [FromBody] UpdateAvatarRequest request)
    {
      return command.Execute(request);
    }

    [HttpGet("get")]
    public OperationResultResponse<ImagesResponse> Get(
      [FromServices] IGetImagesCommand command,
      [FromQuery] Guid entityId,
      [FromQuery] EntityType entityType,
      [FromQuery] bool getCurrentAvatar = false)
    {
      return command.Execute(entityId, entityType, getCurrentAvatar);
    }

    [HttpPost("remove")]
    public OperationResultResponse<bool> Remove(
      [FromServices] IRemoveImagesCommand command,
      [FromBody] RemoveImagesRequest request)
    {
      return command.Execute(request);
    }
  }
}
