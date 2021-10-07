using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class CreateImageDataMapper : ICreateImageDataMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateImageDataMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public List<CreateImageData> Map(List<AddImageRequest> request)
    {
      Guid senderId = _httpContextAccessor.HttpContext.GetUserId();

      if (request == null)
      {
        return null;
      }

      return request.Select(x => new CreateImageData(x.Name, x.Content, x.Extension, senderId)).ToList();
    }

    public List<CreateImageData> Map(string name, string content, string extension)
    {
      return (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(extension))
        ? null
        : new() { new CreateImageData(name, content, extension, _httpContextAccessor.HttpContext.GetUserId()) };
    }
  }
}
