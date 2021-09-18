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
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

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

    private async Task<List<ImageData>> GetImages(List<Guid> imagesIds, List<string> errors)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      const string errorMessage = "Can't get images. Please try again later.";
      const string logMessage = "Errors while getting images with ids: {Ids}.";

      try
      {
        Response<IOperationResult<IGetImagesResponse>> response = await _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imagesIds, ImageSource.User), default, TimeSpan.FromSeconds(5));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.ImagesData;
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join(", ", imagesIds),
          string.Join('\n', response.Message.Errors));

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

    public async Task<OperationResultResponse<ImagesResponse>> Execute(Guid entityId, EntityType entityType)
    {
      OperationResultResponse<ImagesResponse> response = new();

      List<Guid> dbImagesIds = _imageRepository.Get(entityId).Select(x => x.ImageId).ToList();

      if (dbImagesIds == null || !dbImagesIds.Any())
      {
        response.Status = OperationResultStatusType.PartialSuccess;
        response.Errors.Add("Images were not found.");

        return response;
      }

      response.Body = _imagesResponseMapper.Map(
        await GetImages(dbImagesIds, response.Errors));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
