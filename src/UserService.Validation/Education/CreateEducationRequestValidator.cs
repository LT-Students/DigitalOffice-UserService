using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Education
{
    public class CreateEducationRequestValidator : AbstractValidator<CreateEducationRequest>, ICreateEducationRequestValidator
    {
        public CreateEducationRequestValidator(IUserRepository repository)
        {
            RuleFor(education => education.UserId)
                .Must(id => repository.IsUserExist(id));

            RuleFor(education => education.UniversityName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("University name is too long");

            RuleFor(education => education.QualificationName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Qualification name is too long");

            RuleFor(education => education.FormEducation)
                .IsInEnum().WithMessage("Wrong form education.");
        }
    }
}
