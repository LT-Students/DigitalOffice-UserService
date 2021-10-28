using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class ImageController : ControllerBase
  {
    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreateImageCommand command,
      [FromBody] CreateImageRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<ImagesResponse>> GetAsync(
      [FromServices] IGetImagesCommand command,
      [FromQuery] Guid entityId,
      [FromQuery] EntityType entityType)
    {
      return await command.ExecuteAsync(entityId, entityType);
    }

    [HttpPost("remove")]
    public async Task<OperationResultResponse<bool>> RemoveAsync(
      [FromServices] IRemoveImagesCommand command,
      [FromBody] RemoveImagesRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpGet("editavatar")]
    public async Task<OperationResultResponse<Guid?>> EditAvatarAsync(
      [FromServices] IEditAvatarCommand command,
      [FromQuery] Guid userId,
      [FromQuery] Guid imageId)
    {
      return await command.ExecuteAsync(userId, imageId);
    }
  }
}
