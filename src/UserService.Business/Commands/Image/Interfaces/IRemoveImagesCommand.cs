﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces
{
  [AutoInject]
  public interface IRemoveImagesCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(RemoveImagesRequest request);
  }
}
