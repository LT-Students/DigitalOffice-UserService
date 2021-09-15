using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface IAddImagesCommand
  {
    OperationResultResponse<List<Guid>> Execute(AddImagesRequest request);
  }
}
