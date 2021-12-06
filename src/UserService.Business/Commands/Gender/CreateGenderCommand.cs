using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.User.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using LT.DigitalOffice.UserService.Validation.Gender.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.User
{
  public class CreateGenderCommand : ICreateGenderCommand
  {
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbGenderMapper _mapper;
    private readonly IGenderRepository _genderRepository;
    private readonly IAccessValidator _accessValidator;
    private readonly ICreateGenderRequestValidator _validator;

    public CreateGenderCommand(
      IHttpContextAccessor httpContextAccessor,
      IDbGenderMapper mapper,
      IGenderRepository genderRepository,
      IAccessValidator accessValidator,
      ICreateGenderRequestValidator validator)
    {
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _genderRepository = genderRepository;
      _accessValidator = accessValidator;
      _validator = validator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateGenderRequest request)
    {
      OperationResultResponse<Guid?> response = new();

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        response.Errors.Add("Not enough rights.");
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        response.Errors.AddRange(errors);
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      if (_genderRepository.DoesGenderAlreadyExist(request.Name))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Conflict;

        response.Errors.Add("Gender with this name already exists.");
        response.Status = OperationResultStatusType.Failed;

        return response;
      }

      response.Body = await _genderRepository.AddAsync(_mapper.Map(request));
      response.Status = OperationResultStatusType.FullSuccess;

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      if (response.Body == default)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
      }

      return response;
    }
  }
}
