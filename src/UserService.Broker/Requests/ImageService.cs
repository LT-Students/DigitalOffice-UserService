using LT.DigitalOffice.Kernel.BrokerSupport.Helpers;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Enums;
using LT.DigitalOffice.Models.Broker.Models.Image;
using LT.DigitalOffice.Models.Broker.Requests.Image;
using LT.DigitalOffice.Models.Broker.Responses.Image;
using LT.DigitalOffice.UserService.Broker.Requests.Interfaces;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests
{
  public class ImageService : IImageService
  {
    private readonly ILogger<ImageService> _logger;
    private readonly IRequestClient<IGetImagesRequest> _rcGetImages;
    private readonly IRequestClient<ICreateImagesRequest> _rcCreateImages;
    private readonly IImageInfoMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ImageService(
      ILogger<ImageService> logger,
      IRequestClient<IGetImagesRequest> rcGetImages,
      IRequestClient<ICreateImagesRequest> rcCreateImages,
      IImageInfoMapper mapper,
      IHttpContextAccessor httpContextAccessor)
    {
      _logger = logger;
      _rcCreateImages = rcCreateImages;
      _rcGetImages = rcGetImages;
      _mapper = mapper;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<ImageInfo>> GetImagesAsync(List<Guid> imagesIds, List<string> errors, CancellationToken token)
    {
      return imagesIds is null || !imagesIds.Any()
        ? default
        : (await RequestHandler.ProcessRequest<IGetImagesRequest, IGetImagesResponse>(
            _rcGetImages,
            IGetImagesRequest.CreateObj(imagesIds, ImageSource.User),
            errors,
            _logger))
          ?.ImagesData
          .Select(_mapper.Map).ToList();
    }

    public async Task<Guid?> CreateImageAsync(CreateAvatarRequest request, List<string> errors)
    {
      return request is null
        ? null
        : (await _rcCreateImages.ProcessRequest<ICreateImagesRequest, ICreateImagesResponse>(
            ICreateImagesRequest.CreateObj(
              new() { new CreateImageData(request.Name, request.Content, request.Extension) },
              ImageSource.User,
              _httpContextAccessor.HttpContext.GetUserId()),
            errors,
            _logger))
          ?.ImagesIds?.FirstOrDefault();
    }
  }
}
