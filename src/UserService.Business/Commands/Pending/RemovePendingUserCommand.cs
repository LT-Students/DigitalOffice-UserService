using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Pending.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Pending
{
  public class RemovePendingUserCommand : IRemovePendingUserCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly IPendingUserRepository _repository;

    public RemovePendingUserCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IPendingUserRepository repository)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _repository = repository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new();
      response.Body = (await _repository.RemoveAsync(userId)) is not null;
      response.Status = response.Body 
        ? Kernel.Enums.OperationResultStatusType.FullSuccess
        : Kernel.Enums.OperationResultStatusType.Failed;

      return response;
    }
  }
}
