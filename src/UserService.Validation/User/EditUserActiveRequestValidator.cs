using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;

namespace LT.DigitalOffice.UserService.Validation.User
{
  public class EditUserActiveRequestValidator : AbstractValidator<EditUserActiveRequest>, IEditUserActiveRequestValidator
  {
    public EditUserActiveRequestValidator(
      IUserRepository _userRepository,
      IPendingUserRepository _pendingRepository)
    {
      RuleFor(request => request)
        .Cascade(CascadeMode.Stop)
        .MustAsync(async (request, _) =>
          (await _userRepository.GetAsync(request.UserId))?.IsActive != request.IsActive)
        .WithMessage("Error is active value.")
        .MustAsync(async (request, _) =>
          !(request.IsActive && await _pendingRepository.GetAsync(request.UserId) is not null))
        .WithMessage("User already pending.");
    }
  }
}
