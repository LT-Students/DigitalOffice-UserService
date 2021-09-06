using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Avatars;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces
{
  [AutoInject]
  public interface IAddAvatarsCommand
  {
    OperationResultResponse<List<Guid>> Execute(AddAvatarRequest request);
  }
}
