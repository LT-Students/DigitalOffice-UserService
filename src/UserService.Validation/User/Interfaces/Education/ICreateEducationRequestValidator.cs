using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;

namespace LT.DigitalOffice.UserService.Validation.User.Interfaces.Education
{
    [AutoInject]
    public interface ICreateEducationRequestValidator : IValidator<CreateEducationRequest>
    {
    }
}
