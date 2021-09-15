﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface IGetImagesCommand
  {
    OperationResultResponse<ImagesResponse> Execute(Guid entityId, EntityType entityType, bool getCurrentAvatar);
  }
}
