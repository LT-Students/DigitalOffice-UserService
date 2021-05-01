using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces.Education
{
    [AutoInject]
    public interface IEditEducationRequestValidator : IValidator<JsonPatchDocument<EditEducationRequest>>
    {
    }
}
