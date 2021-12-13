using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.UserService.Validation.Gender.Interfaces
{
  [AutoInject]
  public interface ICreateGenderRequestValidator : IValidator<CreateGenderRequest>
  {
  }
}
