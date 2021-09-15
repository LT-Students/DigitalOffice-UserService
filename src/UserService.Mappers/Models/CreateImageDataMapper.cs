using LT.DigitalOffice.Models.Broker.Models;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class CreateImageDataMapper : ICreateImageDataMapper
  {
    public List<CreateImageData> Map(List<AddImageRequest> request, Guid senderId)
    {
      List<CreateImageData> result = new();

      if (request == null)
      {
        result = null;
      }
      else
      {
        foreach (AddImageRequest image in request)
        {
          result.Add(new CreateImageData(image.Name, image.Content, image.Extension, senderId));
        }
      }

      return result;
    }

    public List<CreateImageData> Map(string name, string content, string extension, Guid senderId)
    {
      List<CreateImageData> result = new();

      if (string.IsNullOrEmpty(content) || string.IsNullOrEmpty(extension))
      {
        result = null;
      }
      else
      {
        result.Add(new CreateImageData(name, content, extension, senderId));
      }

      return result;
    }
  }
}
