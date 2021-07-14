using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;

namespace LT.DigitalOffice.UserService.Validation.Communication.Interfaces
{
    [AutoInject]
    public interface ICreateCommunicationRequestValidator : IValidator<CreateCommunicationRequest>
    {
    }
}
