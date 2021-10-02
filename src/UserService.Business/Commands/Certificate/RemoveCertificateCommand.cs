using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.UserService.Business.Commands.Certificate.Interfaces;
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

namespace LT.DigitalOffice.UserService.Business.Commands.Certificate
{
  public class RemoveCertificateCommand : IRemoveCertificateCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IImageRepository _imageRepository;
    private readonly ICertificateRepository _certificateRepository;
    private readonly IRequestClient<IRemoveImagesRequest> _removeImagesRequest;
    private readonly ILogger<RemoveCertificateCommand> _logger;

    public RemoveCertificateCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IUserRepository userRepository,
      IImageRepository imageRepository,
      ICertificateRepository certificateRepository,
      IRequestClient<IRemoveImagesRequest> removeImagesRequest,
      ILogger<RemoveCertificateCommand> logger)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _userRepository = userRepository;
      _imageRepository = imageRepository;
      _certificateRepository = certificateRepository;
      _removeImagesRequest = removeImagesRequest;
      _logger = logger;
    }

    private async Task<bool> RemoveImages(List<Guid> imagesIdsToRemove, ICollection<string> errors)
    {
      string errorMsg = "Can not remove certificate images. Reason: {errors}";

      try
      {
        Response<IOperationResult<bool>> response = await
         _removeImagesRequest.GetResponse<IOperationResult<bool>>(
           IRemoveImagesRequest.CreateObj(imagesIdsToRemove, ImageSource.User));

        IOperationResult<bool> responsedMsg = response.Message;

        if (responsedMsg.IsSuccess && responsedMsg.Body)
        {
          return responsedMsg.Body;
        }

        _logger.LogWarning(errorMsg, string.Join(',', responsedMsg.Errors));
      }
      catch (Exception e)
      {
        _logger.LogError(e, errorMsg);
      }

      errors.Add("Cannot remove certificates images");
      return false;
    }

    public async Task<OperationResultResponse<bool>> Execute(Guid certificateId)
    {
      OperationResultResponse<bool> result = new();
      List<string> resultErrors = result.Errors;

      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();
      DbUser sender = _userRepository.Get(senderId);
      DbUserCertificate userCertificate = _certificateRepository.Get(certificateId);

      if (senderId != userCertificate.UserId && !_accessValidator.HasRights(Rights.AddEditRemoveUsers))
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;

        resultErrors.Add("Not enough rights");
        result.Status = OperationResultStatusType.Failed;

        return result;
      }

      List<Guid> imagesIdsToRemove = _imageRepository.GetImagesIds(userCertificate.Id);

      if (imagesIdsToRemove.Any())
      {
        await RemoveImages(imagesIdsToRemove, resultErrors);
        _imageRepository.Remove(imagesIdsToRemove);
      }

      result.Body = _certificateRepository.Remove(userCertificate);
      result.Status = resultErrors.Any() ? OperationResultStatusType.PartialSuccess : OperationResultStatusType.FullSuccess;

      return result;
    }
  }
}
