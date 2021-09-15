using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Image.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Business.Commands.Image
{
  public class GetImagesCommand : IGetImagesCommand
  {
    private readonly IImageRepository _imageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IImagesResponseMapper _imagesResponseMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly ILogger<GetImagesCommand> _logger;

    private List<ImageData> GetImages(List<Guid> imagesIds, List<string> errors)
    {
      string errorMessage = "Can't get images. Please try again later.";
      string logMessage = "Errors while getting images with ids: {Ids}.";

      try
      {
        IOperationResult<IGetImagesResponse> response = _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imagesIds, ImageSource.User))
          .Result.Message;

        if (response.IsSuccess)
        {
          return response.Body.ImagesData;
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join(", ", imagesIds),
          string.Join('\n', response.Errors));

        errors.Add(errorMessage);
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imagesIds));

        errors.Add(errorMessage);
      }

      return null;
    }

    public GetImagesCommand(
      IImageRepository imageRepository,
      IUserRepository userRepository,
      IImagesResponseMapper imagesResponseMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IGetImagesRequest> rcGetImages,
      ILogger<GetImagesCommand> logger)
    {
      _imageRepository = imageRepository;
      _userRepository = userRepository;
      _imagesResponseMapper = imagesResponseMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcGetImages = rcGetImages;
      _logger = logger;
    }

    public OperationResultResponse<ImagesResponse> Execute(Guid entityId, EntityType entityType, bool getCurrentAvatar)
    {
      OperationResultResponse<ImagesResponse> response = new();

      /*Guid? userId = null;
      switch (entityType)
      {
        case EntityType.User:
          userId = request.EntityId;
          break;

        case EntityType.Certificate:
          userId = _certificateRepository.Get(request.EntityId).UserId;
          break;

        case EntityType.Education:
          userId = _educationRepository.Get(request.EntityId).UserId;
          break;
      }

      DbUser dbUser = _userRepository.Get(userId);

      if (dbUser == null)
      {
        _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        response.Status = OperationResultStatusType.Failed;
        response.Errors.Add("User was not found.");

        return response;
      }*/

      List<Guid> dbImagesIds = null;

      if (getCurrentAvatar)
      {
        DbUser dbUser = _userRepository.Get(entityId);

        if (dbUser != null)
        {
          dbImagesIds.Add(dbUser.AvatarFileId.Value);
        }
      }
      else
      {
        dbImagesIds = _imageRepository.Get(entityId).Select(x => x.ImageId).ToList();
      }

      if (dbImagesIds == null || !dbImagesIds.Any())
      {
        response.Status = OperationResultStatusType.PartialSuccess;
        response.Errors.Add("Images were not found.");

        return response;
      }

      response.Body = _imagesResponseMapper.Map(
        GetImages(dbImagesIds, response.Errors));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
