using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;
using LT.DigitalOffice.UserService.Validation.Gender.Resources;
using System.Globalization;
using System.Threading;

namespace LT.DigitalOffice.UserService.Validation.Gender
{
  public class CreateGenderRequestValidator : AbstractValidator<CreateGenderRequest>, ICreateGenderRequestValidator
  {
    public CreateGenderRequestValidator(IGenderRepository genderRepository)
    {
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

      RuleFor(gender => gender.Name)
        .MustAsync(async (name, _) => !await genderRepository.DoesGenderAlreadyExistAsync(name))
        .WithMessage(CreateGenderRequestValidationResource.NameExists);
    }
  }
}
