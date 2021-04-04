using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces
{
    public interface ICreateUserRequestValidator : IValidator<CreateUserRequest>
    {
    }
}
