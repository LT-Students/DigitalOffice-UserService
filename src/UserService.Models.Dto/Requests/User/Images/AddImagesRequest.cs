﻿using LT.DigitalOffice.UserService.Models.Dto.Enums;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Models.Dto.Requests.User.Images
{
  public record AddImagesRequest
  {
    public Guid EntityId { get; set; }
    public EntityType EntityType { get; set; }
    public List<AddImageRequest> Images { get; set; }
  }
}
