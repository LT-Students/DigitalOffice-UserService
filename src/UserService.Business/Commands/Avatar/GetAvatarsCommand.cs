using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Avatar;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class GetAvatarsCommand : IGetAvatarsCommand
  {
    private readonly IAvatarRepository _avatarRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAvatarsResponseMapper _avatarsResponseMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly ILogger<GetAvatarsCommand> _logger;

    private List<ImageData> GetImages(List<Guid> imageIds, List<string> errors)
    {
      string errorMessage = "Can't get images. Please try again later.";
      string logMessage = "Errors while getting images with ids: {Ids}.";

      try
      {
        Response<IOperationResult<IGetImagesResponse>> getResponse = _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imageIds, ImageSource.User))
          .Result;

        if (getResponse.Message.IsSuccess)
        {
          return getResponse.Message.Body.ImagesData;
        }

        string warningMessage = logMessage + "Errors: {Errors}";

        _logger.LogWarning(
          warningMessage,
          string.Join(", ", imageIds),
          string.Join('\n', getResponse.Message.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imageIds));

        errors.Add(errorMessage);
      }

      return null;
    }

    public GetAvatarsCommand(
      IAvatarRepository avatarRepository,
      IUserRepository userRepository,
      IAvatarsResponseMapper avatarsResponseMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IGetImagesRequest> rcGetImages,
      ILogger<GetAvatarsCommand> logger)
    {
      _avatarRepository = avatarRepository;
      _userRepository = userRepository;
      _avatarsResponseMapper = avatarsResponseMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcGetImages = rcGetImages;
      _logger = logger;
    }

    public OperationResultResponse<AvatarsResponse> Execute(Guid userId, bool getOnlyCurrent)
    {
      OperationResultResponse<AvatarsResponse> response = new();
      DbUser dbUser = _userRepository.Get(userId);

      if (dbUser == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("User was not found.");

        return response;
      }

      List<Guid> dbImagesIds = null;

      if (getOnlyCurrent && dbUser.AvatarFileId != null)
      {
        dbImagesIds.Add(dbUser.AvatarFileId.Value);
      }
      else if (!getOnlyCurrent)
      {
        dbImagesIds = _avatarRepository.Get(userId).Select(x => x.ImageId).ToList();
      }

      if (dbImagesIds == null || !dbImagesIds.Any())
      {
        response.Status = OperationResultStatusType.PartialSuccess;
        response.Errors.Add("User images was not found.");

        return response;
      }

      response.Body = _avatarsResponseMapper.Map(
          GetImages(dbImagesIds, response.Errors),
          dbUser.AvatarFileId);

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
