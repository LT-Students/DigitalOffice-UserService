using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces
{
  [AutoInject]
  public interface IEditUserActiveRequestValidator : IValidator<EditUserActiveRequest>
  {
  }
}
