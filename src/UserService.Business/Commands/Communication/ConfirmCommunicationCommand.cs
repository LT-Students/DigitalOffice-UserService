using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class ConfirmCommunicationCommand : IConfirmCommunicationCommand
  {
    private readonly IUserCommunicationRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly IResponseCreator _responseCreator;

    public ConfirmCommunicationCommand(
      IUserCommunicationRepository repository,
      IMemoryCache cache,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _cache = cache;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid communicationId, string secret)
    {
      if (!_cache.TryGetValue(communicationId, out string value) || value != secret)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest);
      }

      OperationResultResponse<bool> response = new();

      response.Body = await _repository.Confirm(communicationId);

      _cache.Remove(communicationId);

      return response;
    }
  }
}
