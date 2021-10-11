using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User.Education;
using LT.DigitalOffice.UserService.Validation.Education.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
  public class EditEducationCommand : IEditEducationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IPatchDbUserEducationMapper _mapper;
    private readonly IEditEducationRequestValidator _validator;

    public EditEducationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IEducationRepository educationRepository,
      IPatchDbUserEducationMapper mapper,
      IEditEducationRequestValidator validator)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _educationRepository = educationRepository;
      _mapper = mapper;
      _validator = validator;
    }

    public async Task<OperationResultResponse<bool>> Execute(Guid educationId, JsonPatchDocument<EditEducationRequest> request)
    {
      var senderId = _httpContextAccessor.HttpContext.GetUserId();
      var dbUser = _userRepository.Get(senderId);
      DbUserEducation userEducation = _educationRepository.Get(educationId);

      if (!(dbUser.IsAdmin ||
        _accessValidator.HasRights(Rights.AddEditRemoveUsers))
        && senderId != userEducation.UserId)
      {
        throw new ForbiddenException("Not enough rights.");
      }

      _validator.ValidateAndThrowCustom(request);

      JsonPatchDocument<DbUserEducation> dbRequest = _mapper.Map(request);

      bool result = await _educationRepository.Edit(userEducation, dbRequest);

      return new OperationResultResponse<bool>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = result
      };
    }
  }
}
