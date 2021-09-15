﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using System;

namespace LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface IUpdateAvatarCommand
  {
    OperationResultResponse<Guid?> Execute(UpdateAvatarRequest request);
  }
}
