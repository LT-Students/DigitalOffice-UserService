using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Education
{
  public class CreateEducationRequestValidator : AbstractValidator<CreateEducationRequest>, ICreateEducationRequestValidator
  {
    public CreateEducationRequestValidator(
      IUserRepository repository,
      IAddImageRequestValidator imageValidator)
    {
      RuleFor(education => education.UserId)
        .Must(id => repository.IsUserExist(id))
        .WithMessage("User dont exist.");

      RuleFor(education => education.UniversityName)
        .NotEmpty().WithMessage("University name must not be empty.")
        .MaximumLength(100)
        .WithMessage("University name is too long.");

      RuleFor(education => education.QualificationName)
        .NotEmpty().WithMessage("Qualification name must not be empty.")
        .MaximumLength(100)
        .WithMessage("Qualification name is too long.");

      RuleFor(education => education.FormEducation)
        .IsInEnum().WithMessage("Wrong form education.");

      When(
        x => (x.Images != null),
        () =>
          RuleForEach(i => i.Images)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Image cannot be null.")
            .SetValidator(imageValidator)
        );
    }
  }
}
