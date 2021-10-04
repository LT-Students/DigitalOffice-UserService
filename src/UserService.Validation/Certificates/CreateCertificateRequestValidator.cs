using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Certificates;
using LT.DigitalOffice.UserService.Validation.Image.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Certificates
{
  public class CreateCertificateRequestValidator : AbstractValidator<CreateCertificateRequest>
  {
    public CreateCertificateRequestValidator(
      IUserRepository repository,
      IAddImageRequestValidator imageValidator)
    {
      RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Certificate name must not be empty.")
        .MaximumLength(100)
        .WithMessage("Certificate name is too long.");

      RuleFor(x => x.SchoolName)
        .NotEmpty().WithMessage("School name must not be empty.")
        .MaximumLength(100)
        .WithMessage("School name is too long.");

      RuleFor(x => x.EducationType)
        .IsInEnum()
        .WithMessage("Wrong education type value.");

      RuleFor(x => x.UserId)
        .Must(id => repository.Get(id) != null)
        .WithMessage("The user do not exist.");

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
