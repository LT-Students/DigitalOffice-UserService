using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Broker.Publishes.Interfaces;
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
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IPublish _publish;

    public RemovePendingUserCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IPendingUserRepository repository,
      IGlobalCacheRepository globalCache,
      IPublish publish)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _repository = repository;
      _publish = publish;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid userId)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      OperationResultResponse<bool> response = new();
      response.Body = (await _repository.RemoveAsync(userId)) is not null;

      if (response.Body)
      {
        await _publish.DisactivateUserAsync(userId);

        await _globalCache.RemoveAsync(userId);
      }

      return response.Body
        ? response
        : _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
    }
  }
}
