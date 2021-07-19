using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.UserService.Validation.Email.Interfaces
{
    [AutoInject]
    public interface IEmailValidator : IValidator<string>
    {
    }
}
