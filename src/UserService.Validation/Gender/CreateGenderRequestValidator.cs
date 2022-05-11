using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Gender
{
  public class CreateGenderRequestValidator : AbstractValidator<CreateGenderRequest>, ICreateGenderRequestValidator
  {
    public CreateGenderRequestValidator(IGenderRepository genderRepository)
    {
      RuleFor(gender => gender.Name)
        .MustAsync(async (name, _) => !await genderRepository.DoesGenderAlreadyExistAsync(name))
        .WithMessage("Gender with this name already exists.");
    }
  }
}
