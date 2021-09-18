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
    [HttpPost("add")]
    public async Task<OperationResultResponse<List<Guid>>> Add(
      [FromServices] IAddImagesCommand command,
      [FromBody] AddImagesRequest request)
    {
      return await command.Execute(request);
    }

    [HttpGet("get")]
    public async Task<OperationResultResponse<ImagesResponse>> Get(
      [FromServices] IGetImagesCommand command,
      [FromQuery] Guid entityId,
      [FromQuery] EntityType entityType,
      [FromQuery] bool isCurrentAvatar = false)
    {
      return await command.Execute(entityId, entityType, isCurrentAvatar);
    }

    [HttpPost("remove")]
    public async Task<OperationResultResponse<bool>> Remove(
      [FromServices] IRemoveImagesCommand command,
      [FromBody] RemoveImagesRequest request)
    {
      return await command.Execute(request);
    }
  }
}
