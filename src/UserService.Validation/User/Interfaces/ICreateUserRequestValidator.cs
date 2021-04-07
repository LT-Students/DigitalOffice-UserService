using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces
{
    [AutoInject]
    public interface ICreateUserRequestValidator : IValidator<CreateUserRequest>
    {
    }
}
