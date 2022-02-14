using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Communication.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Communication;
using LT.DigitalOffice.UserService.Validation.Communication.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Communication
{
  public class EditCommunicationCommand : IEditCommunicationCommand
  {
    private readonly ICommunicationRepository _repository;
    private readonly IAccessValidator _accessValidator;
    private readonly IPatchDbUserCommunicationMapper _mapper;
    private readonly IEditCommunicationRequestValidator _validator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IResponseCreator _responseCreator;

    public EditCommunicationCommand(
      ICommunicationRepository repository,
      IAccessValidator accessValidator,
      IPatchDbUserCommunicationMapper mapper,
      IEditCommunicationRequestValidator validator,
      IHttpContextAccessor httpContextAccessor,
      IResponseCreator responseCreator)
    {
      _repository = repository;
      _accessValidator = accessValidator;
      _mapper = mapper;
      _validator = validator;
      _httpContextAccessor = httpContextAccessor;
      _responseCreator = responseCreator;
    }
    public async Task<OperationResultResponse<bool>> ExecuteAsync(
      Guid communicationId,
      JsonPatchDocument<EditCommunicationRequest> patch)
    {
      DbUserCommunication dbUserCommunication = await _repository
        .GetAsync(communicationId);

      if (dbUserCommunication is null)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound);
      }

      if (_httpContextAccessor.HttpContext.GetUserId() != dbUserCommunication.UserId &&
        !await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator
        .ValidateAsync((dbUserCommunication, patch));

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(vf => vf.ErrorMessage).ToList());
      }

      OperationResultResponse<bool> response = new();

      if (patch.Operations[0].path
        .EndsWith(nameof(EditCommunicationRequest.Type), StringComparison.OrdinalIgnoreCase))
      {
        await _repository.RemoveBaseTypeAsync(dbUserCommunication.UserId);
      }

      response.Body = await _repository.EditAsync(dbUserCommunication, _mapper.Map(patch));

      return response;
    }
  }
}
