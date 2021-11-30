using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.UserService.Validation.Password.Interfaces
{
  [AutoInject]
  public interface IPasswordValidator : IValidator<string>
  {
  }
}
