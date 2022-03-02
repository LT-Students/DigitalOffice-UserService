using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;

namespace LT.DigitalOffice.UserService.Validation.Communication.Interfaces
{
  [AutoInject]
  public interface IEditCommunicationRequestValidator : IValidator<(
    DbUserCommunication dbUserCommunication,
    EditCommunicationRequest request)>
  {
  }
}
