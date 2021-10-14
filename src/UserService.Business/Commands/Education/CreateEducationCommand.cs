using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
  public class CreateEducationCommand : ICreateEducationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IDbUserEducationMapper _mapper;
    private readonly IEducationRepository _educationRepository;
    private readonly ICreateEducationRequestValidator _validator;

    public CreateEducationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IDbUserEducationMapper mapper,
      IEducationRepository educationRepository,
      ICreateEducationRequestValidator validator)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _mapper = mapper;
      _educationRepository = educationRepository;
      _validator = validator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreateEducationRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers))
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

      DbUserEducation dbEducation = _mapper.Map(request);

      await _educationRepository.AddAsync(dbEducation);

      return new OperationResultResponse<Guid?>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = dbEducation.Id
      };
    }
  }
}
