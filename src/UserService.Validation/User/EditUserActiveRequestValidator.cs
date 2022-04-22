using FluentValidation;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using System.Linq;

namespace LT.DigitalOffice.UserService.Validation.User
{
  public class EditUserActiveRequestValidator : AbstractValidator<(DbUser dbUser, EditUserActiveRequest request)>, IEditUserActiveRequestValidator
  {
    public EditUserActiveRequestValidator(
      IPendingUserRepository _pendingRepository)
    {
      RuleFor(x => x)
        .Cascade(CascadeMode.Stop)
        //.Must(x => x.dbUser.IsActive != x.request.IsActive)
        //.WithMessage("Error is active value.")
        .MustAsync(async (x, _) =>
          !(x.request.IsActive && await _pendingRepository.GetAsync(x.request.UserId) is not null))
        .WithMessage("User already pending.")
        .Must(x =>
        {
          if (x.request.IsActive && x.request.CommunicationId.HasValue)
          {
            DbUserCommunication uc = x.dbUser.Communications.FirstOrDefault(c => c.Id == x.request.CommunicationId);
            return uc is not null && (uc.Type == (int)CommunicationType.Email || uc.Type == (int)CommunicationType.BaseEmail);
          }

          return !x.request.IsActive;
        })
        .WithMessage("Wrong user communication.");
    }
  }
}
