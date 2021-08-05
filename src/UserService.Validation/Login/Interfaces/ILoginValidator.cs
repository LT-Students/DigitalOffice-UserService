using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;

namespace LT.DigitalOffice.UserService.Validation.Login.Interfaces
{
    [AutoInject]
    public interface ILoginValidator : IValidator<string>
    {

    }
}
