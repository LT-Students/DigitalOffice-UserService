using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Education
{
  public class CreateEducationRequestValidator : AbstractValidator<CreateEducationRequest>, ICreateEducationRequestValidator
  {
    public CreateEducationRequestValidator(IUserRepository _userRepository)
    {
      RuleFor(education => education.UserId)
        .MustAsync(async (e, token, context) => await _userRepository.IsUserExistAsync(e.UserId));

      RuleFor(education => education.UniversityName)
        .NotEmpty().WithMessage("University name must not be empty.")
        .MaximumLength(100).WithMessage("University name is too long");

      RuleFor(education => education.QualificationName)
        .NotEmpty().WithMessage("Qualification name must not be empty.")
        .MaximumLength(100).WithMessage("Qualification name is too long");

      RuleFor(education => education.FormEducation)
        .IsInEnum().WithMessage("Wrong form education.");
    }
  }
}
