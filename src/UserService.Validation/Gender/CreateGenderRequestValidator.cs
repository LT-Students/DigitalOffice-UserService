using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Gender
{
  public class CreateGenderRequestValidator : AbstractValidator<CreateGenderRequest>, ICreateGenderRequestValidator
  {
    public CreateGenderRequestValidator(IGenderRepository genderRepository)
    {
      RuleFor(s => s.Name.Trim())
        .NotEmpty().WithMessage("Gender must not be empty.")
        .Must(name => !genderRepository.DoesGenderAlreadyExist(name))
        .WithMessage("Gender with this name already exists.");
    }
  }
}
