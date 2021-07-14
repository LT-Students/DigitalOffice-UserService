using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.UserService.Validation.Communication.Interfaces
{
    [AutoInject]
    public interface IEditCommunicationRequestValidator : IValidator<JsonPatchDocument<EditCommunicationRequest>>
    {
    }
}
