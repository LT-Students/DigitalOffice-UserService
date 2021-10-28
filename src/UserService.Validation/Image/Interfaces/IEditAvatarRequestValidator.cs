using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images;
using System;

namespace LT.DigitalOffice.UserService.Validation.Image.Interfaces
{
  [AutoInject]
  public interface IEditAvatarRequestValidator : IValidator<(Guid userId, Guid imageId)>
  {
  }
}
