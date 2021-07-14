using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
    public class CreateCommunicationRequestValidator : AbstractValidator<CreateCommunicationRequest>, ICreateCommunicationRequestValidator
    {
        public CreateCommunicationRequestValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Value)
                .NotEmpty();

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Incorrect communication type format.");

            RuleFor(x => x.UserId)
                .NotNull()
                .Must(id => userRepository.IsUserExist(id.Value))
                .WithMessage("The user must exist");
        }
    }
}
