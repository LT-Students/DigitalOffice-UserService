using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Patch.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
  public class EditEducationCommand : IEditEducationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEducationRepository _educationRepository;
    private readonly IPatchDbUserEducationMapper _mapper;
    private readonly IEditEducationRequestValidator _validator;
    private readonly IResponseCreater _responseCreater;

    public EditEducationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IEducationRepository educationRepository,
      IPatchDbUserEducationMapper mapper,
      IEditEducationRequestValidator validator,
      IResponseCreater responseCreater)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _educationRepository = educationRepository;
      _mapper = mapper;
      _validator = validator;
      _responseCreater = responseCreater;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid educationId, JsonPatchDocument<EditEducationRequest> request)
    {
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      DbUserEducation userEducation = _educationRepository.Get(educationId);

      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemoveUsers)
        && senderId != userEducation.UserId)
      {
        return _responseCreater.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      _validator.ValidateAndThrowCustom(request);

      JsonPatchDocument<DbUserEducation> dbRequest = _mapper.Map(request);

      bool result = await _educationRepository.EditAsync(userEducation, dbRequest);

      return new OperationResultResponse<bool>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = result
      };
    }
  }
}
