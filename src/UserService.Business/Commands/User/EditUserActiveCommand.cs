using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.UserService.Business.Commands.Password.Interfaces;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.User.Interfaces;
using MassTransit;
using Microsoft.AspNetCore.Http;
using FluentValidation.Results;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  /// <inheritdoc/>
  public class EditUserActiveCommand : IEditUserActiveCommand
  {
    private readonly IEditUserActiveRequestValidator _validator;
    private readonly IUserRepository _userRepository;
    private readonly IPendingUserRepository _pendingRepository;
    private readonly IUserCommunicationRepository _communicationRepository;
    private readonly IGeneratePasswordCommand _generatePassword;
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;
    private readonly IBus _bus;

    public EditUserActiveCommand(
      IEditUserActiveRequestValidator validator,
      IUserRepository userRepository,
      IPendingUserRepository pendingRepository,
      IUserCommunicationRepository communicationRepository,
      IGeneratePasswordCommand generatePassword,
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator,
      IBus bus)
    {
      _validator = validator;
      _userRepository = userRepository;
      _pendingRepository = pendingRepository;
      _communicationRepository = communicationRepository;
      _generatePassword = generatePassword;
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
      _bus = bus;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditUserActiveRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers)
        || _httpContextAccessor.HttpContext.GetUserId() == request.UserId)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator
        .ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      if (!request.IsActive)
      {
        response.Body = await _userRepository.SwitchActiveStatusAsync(request.UserId, request.IsActive);

        await _bus.Publish<IDisactivateUserRequest>(IDisactivateUserRequest.CreateObj(
          request.UserId,
          _httpContextAccessor.HttpContext.GetUserId()));
      }
      else
      {
        await _pendingRepository.CreateAsync(
          new DbPendingUser()
          {
            UserId = request.UserId,
            Password = _generatePassword.Execute(),
            CommunicationId = request.CommunicationId.HasValue 
              ? request.CommunicationId.Value
              : (await _communicationRepository.GetBaseAsync(request.UserId)).Id
          });

        response.Body = true;
      }

      return response;
    }
  }
}