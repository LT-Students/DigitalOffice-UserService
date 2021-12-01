using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Business.Commands.Avatar.Interfaces;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Responses.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Responses.Image;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Business.Commands.Avatar
{
  public class GetAvatarsCommand : IGetAvatarsCommand
  {
    private readonly IAvatarRepository _imageRepository;
    private readonly IImagesResponseMapper _imagesResponseMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly ILogger<GetAvatarsCommand> _logger;
    private readonly IResponseCreator _responseCreator;

    private async Task<List<ImageData>> GetImages(List<Guid> imagesIds, List<string> errors)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      const string logMessage = "Errors while getting images with ids: {Ids}.";

      try
      {
        Response<IOperationResult<IGetImagesResponse>> response = await _rcGetImages.GetResponse<IOperationResult<IGetImagesResponse>>(
          IGetImagesRequest.CreateObj(imagesIds, ImageSource.User));

        if (response.Message.IsSuccess)
        {
          return response.Message.Body.ImagesData;
        }

        _logger.LogWarning(
          logMessage + " Errors: {Errors}",
          string.Join(", ", imagesIds),
          string.Join('\n', response.Message.Errors));
      }
      catch (Exception e)
      {
        _logger.LogError(e, logMessage, string.Join(", ", imagesIds));
      }
      errors.Add("Can't get images. Please try again later.");

      return null;
    }

    public GetAvatarsCommand(
      IAvatarRepository imageRepository,
      IImagesResponseMapper imagesResponseMapper,
      IHttpContextAccessor httpContextAccessor,
      IRequestClient<IGetImagesRequest> rcGetImages,
      ILogger<GetAvatarsCommand> logger,
      IResponseCreator responseCreator)
    {
      _imageRepository = imageRepository;
      _imagesResponseMapper = imagesResponseMapper;
      _httpContextAccessor = httpContextAccessor;
      _rcGetImages = rcGetImages;
      _logger = logger;
      _responseCreator = responseCreator;
    }

    public async Task<OperationResultResponse<ImagesResponse>> ExecuteAsync(Guid userId)
    {
      List<Guid> dbImagesIds = await _imageRepository.GetAvatarsByUserId(userId);

      if (dbImagesIds == null || !dbImagesIds.Any())
      {
        return _responseCreator.CreateFailureResponse<ImagesResponse>(HttpStatusCode.NotFound);
      }

      OperationResultResponse<ImagesResponse> response = new();

      response.Body = _imagesResponseMapper.Map(
        await GetImages(dbImagesIds, response.Errors));

      response.Status = response.Errors.Any()
        ? OperationResultStatusType.PartialSuccess
        : OperationResultStatusType.FullSuccess;

      return response;
    }
  }
}
