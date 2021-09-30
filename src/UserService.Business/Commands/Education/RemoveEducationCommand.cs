using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Education.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Education
{
  public class RemoveEducationCommand : IRemoveEducationCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IEducationRepository _educationRepository;
    private readonly IRequestClient<IRemoveImagesRequest> _removeImagesRequest;
    private readonly ILogger<RemoveEducationCommand> _logger;

    public RemoveEducationCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IEducationRepository educationRepository,
      IRequestClient<IRemoveImagesRequest> removeImagesRequest,
      ILogger<RemoveEducationCommand> logger)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _educationRepository = educationRepository;
      _logger = logger;
      _removeImagesRequest = removeImagesRequest;
    }

    private async Task<bool> RemoveImages(ICollection<DbUserEducationImage> imagesToRemove, List<string> errors)
    {
      string errorMsg = "Can not remove education images. Reason: {errors}";

      try
      {
        Response<IOperationResult<bool>> response = await
          _removeImagesRequest.GetResponse<IOperationResult<bool>>(
          IRemoveImagesRequest.CreateObj(imagesToRemove.Select(i => i.ImageId).ToList(), ImageSource.User));

        IOperationResult<bool> responsedMsg = response.Message;

        if (responsedMsg.IsSuccess)
        {
          return responsedMsg.Body;
        }

        _logger.LogWarning(errorMsg, string.Join(',', responsedMsg.Errors));
      }
      catch (Exception e)
      {
        _logger.LogError(e, errorMsg);
      }

      errors.Add("Cannot remove education images");
      return false;
    }

    public async Task<OperationResultResponse<bool>> Execute(Guid educationId)
    {
      OperationResultResponse<bool> result = new();
      List<string> resultErrors = result.Errors;

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      DbUser sender = _userRepository.Get(senderId);
      DbUserEducation userEducation = _educationRepository.Get(educationId);

      if (!(sender.IsAdmin || _accessValidator.HasRights(Rights.AddEditRemoveUsers)) 
        && senderId != userEducation.UserId)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        resultErrors.Add("Not enough rights");
        result.Status = OperationResultStatusType.Failed;

        return result;
      }

      ICollection<DbUserEducationImage> imagesToRemove = userEducation.Images;

      if (imagesToRemove.Any())
      {
        await RemoveImages(imagesToRemove, resultErrors);
      }

      result.Body = _educationRepository.Remove(userEducation);
      result.Status = resultErrors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;
      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;

      return result;
    }
  }
}
