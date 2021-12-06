using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Gender
{
  public class CreateGenderRequestValidator : AbstractValidator<CreateGenderRequest>, ICreateGenderRequestValidator
  {
    public CreateGenderRequestValidator()
    {
      RuleFor(s => s.Name.Trim())
        .NotEmpty();
    }
  }
}
