using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using System.Text.RegularExpressions;

namespace LT.DigitalOffice.UserService.Validation.Communication
{
    public class CreateCommunicationRequestValidator : AbstractValidator<CreateCommunicationRequest>, ICreateCommunicationRequestValidator
    {
        private static Regex PhoneRegex = new(@"\d");
        private static Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        public CreateCommunicationRequestValidator(IUserRepository userRepository)
        {
            RuleFor(x => x.Value)
                .NotEmpty();

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Incorrect communication type format.");

            When(x => x.Type == CommunicationType.Phone, () => 
                RuleFor(x => x.Value)
                    .Must(v => PhoneRegex.IsMatch(v.Trim())));

            When(x => x.Type == CommunicationType.Email, () =>
                RuleFor(x => x.Value)
                    .Must(v => EmailRegex.IsMatch(v.Trim())));

            RuleFor(x => x.UserId)
                .NotNull()
                .Must(id => userRepository.IsUserExist(id.Value))
                .WithMessage("The user must exist");
        }
    }
}
