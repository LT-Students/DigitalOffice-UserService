using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces
{
    [AutoInject]
    public interface IEditUserRequestValidator : IValidator<JsonPatchDocument<EditUserRequest>>
    {
    }
}