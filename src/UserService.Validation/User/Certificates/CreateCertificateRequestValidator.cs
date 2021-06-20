using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;

namespace LT.DigitalOffice.UserService.Validation.User.Certificates
{
    public class CreateCertificateRequestValidator : AbstractValidator<CreateCertificateRequest>
    {
        public CreateCertificateRequestValidator(IUserRepository repository)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Name of Certificate is too long");

            RuleFor(x => x.SchoolName)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Name of school is too long");

            RuleFor(x => x.EducationType)
                .IsInEnum()
                .WithMessage("Wrong education type value.");

            RuleFor(x => x.UserId)
                .Must(id => repository.Get(id) != null)
                .WithMessage("The user must exist");

            RuleFor(x => x.Image)
                .NotNull();
        }
    }
}
