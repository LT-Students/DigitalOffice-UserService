using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using LT.DigitalOffice.UserService.Validation.Achievement.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement
{
  public class EditAchievementCommand : IEditAchievementCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IAchievementRepository _repository;
    private readonly IPatchDbAchievementMapper _mapper;
    private readonly IEditAchievementRequestValidator _validator;

    public EditAchievementCommand(
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IAchievementRepository repository,
      IPatchDbAchievementMapper mapper,
      IEditAchievementRequestValidator validator)
    {
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _repository = repository;
      _mapper = mapper;
      _validator = validator;
    }

    public OperationResultResponse<bool> Execute(Guid achievementId, JsonPatchDocument<EditAchievementRequest> request)
    {
      if (!_accessValidator.HasRights(Rights.AddEditRemoveUsers))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<bool>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      OperationResultResponse<bool> response = new();

      response.Body = _repository.Edit(achievementId, _mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      if (!response.Body)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
      }

      return response;
    }
  }
}