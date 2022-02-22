using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class ImageService : IImageService
  {
    private readonly ILogger<ImageService> _logger;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IImageInfoMapper _mapper;

    public ImageService(
      ILogger<ImageService> logger,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IImageInfoMapper mapper)
    {
      _logger = logger;
      _rcGetImages = rcGetImages;
      _mapper = mapper;
    }

    public async Task<List<ImageInfo>> GetImagesAsync(List<Guid> imageIds, List<string> errors)
    {
      return imageIds is null || !imageIds.Any()
        ? default
        : (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
            _rcGetImages,
            IGetImagesRequest.CreateObj(imageIds, ImageSource.User),
            errors,
            _logger))
          ?.ImagesData
          .Select(_mapper.Map).ToList();
    }
  }
}
