using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Validation.Password.Interfaces
{
  [AutoInject]
  public interface IChangePassordRequestValidator : IValidator<ChangePasswordRequest>
  {
  }
}
