﻿using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class ImageRepository : IImageRepository
  {
    private readonly IDataProvider _provider;

    public ImageRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<List<Guid>> Create(List<DbEntityImage> dbEntityImages)
    {
      if (dbEntityImages == null || !dbEntityImages.Any() || dbEntityImages.Contains(null))
      {
        return null;
      }

      _provider.EntitiesImages.AddRange(dbEntityImages);
      await _provider.SaveAsync();

      return dbEntityImages.Select(x => x.ImageId).ToList();
    }

    public List<Guid> GetImagesIds(Guid entityId)
    {
      return _provider.EntitiesImages.Where(x => x.EntityId == entityId).Select(x => x.ImageId).ToList();
    }

    public List<DbEntityImage> Get(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return null;
      }

      return _provider.EntitiesImages.Where(x => imagesIds.Contains(x.ImageId)).ToList();
    }

    public async Task<bool> Remove(List<Guid> imagesIds)
    {
      if (imagesIds == null || !imagesIds.Any())
      {
        return false;
      }

      List<DbEntityImage> removeUsersAvatars = Get(imagesIds);

      _provider.EntitiesImages.RemoveRange(removeUsersAvatars);
      await _provider.SaveAsync();

      return true;
    }
  }
}
