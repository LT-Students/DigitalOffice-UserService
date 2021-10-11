using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Achievement.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Achievement;
using LT.DigitalOffice.UserService.Validation.Achievement.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Achievement
{
  public class CreateAchievementCommand : ICreateAchievementCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAccessValidator _accessValidator;
    private readonly IAchievementRepository _repository;
    private readonly IDbAchievementMapper _mapper;
    private readonly ICreateAchievementRequestValidator _validator;

    public CreateAchievementCommand(
      IHttpContextAccessor httpContextAccessor,
      IAccessValidator accessValidator,
      IAchievementRepository repository,
      IDbAchievementMapper mapper,
      ICreateAchievementRequestValidator validator)
    {
      _httpContextAccessor = httpContextAccessor;
      _accessValidator = accessValidator;
      _repository = repository;
      _mapper = mapper;
      _validator = validator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateAchievementRequest request)
    {
      if (!_accessValidator.HasRights(Rights.AddEditRemoveUsers))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        return new OperationResultResponse<Guid?>
        {
          Status = OperationResultStatusType.Failed,
          Errors = new() { "Not enough rights." }
        };
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        return new OperationResultResponse<Guid?>
        {
          Status = OperationResultStatusType.Failed,
          Errors = errors
        };
      }

      OperationResultResponse<Guid?> response = new();

      response.Body = await _repository.CreateAsync(_mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      if (response.Body == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Status = OperationResultStatusType.Failed;
      }

      return response;
    }
  }
}
