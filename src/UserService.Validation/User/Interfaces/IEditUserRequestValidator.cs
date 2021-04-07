using FluentValidation;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces
{
    public interface IEditUserRequestValidator : IValidator<JsonPatchDocument<EditUserRequest>>
    {
    }
}