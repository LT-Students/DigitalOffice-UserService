using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Exceptions.Models;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
  public class RemoveEducationCommand : IRemoveEducationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IEducationRepository _educationRepository;

    public RemoveEducationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IEducationRepository educationRepository)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _educationRepository = educationRepository;
    }

    public async Task<OperationResultResponse<bool>> Execute(Guid educationId)
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

      bool result = await _educationRepository.Remove(userEducation);

      return new OperationResultResponse<bool>
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = result
      };
    }
  }
}
