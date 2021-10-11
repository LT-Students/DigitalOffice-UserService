using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class RemoveCommunicationCommand : IRemoveCommunicationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly ICommunicationRepository _communicationRepository;

    public RemoveCommunicationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      ICommunicationRepository communicationRepository)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _communicationRepository = communicationRepository;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid communicationId)
    {
      var senderId = _httpContextAccessor.HttpContext.GetUserId();
      var sender = _userRepository.Get(senderId);
      DbUserCommunication userCommunication = _communicationRepository.Get(communicationId);

      if (!(sender.IsAdmin ||
        _accessValidator.HasRights(Rights.AddEditRemoveUsers))
        && senderId != userCommunication.UserId)
      {
        throw new ForbiddenException("Not enough rights.");
      }

      bool result = await _communicationRepository.RemoveAsync(userCommunication);

      return new OperationResultResponse<bool>
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result,
        Errors = new()
      };
    }
  }
}
