﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Avatar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface IImageService
  {
    Task<List<ImageInfo>> GetImagesAsync(List<Guid> imageIds, List<string> errors);

    Task<Guid?> CreateImageAsync(CreateAvatarRequest request, List<string> errors);

    Task<bool> RemoveImages(List<Guid> imagesIds, List<string> errors);
  }
}