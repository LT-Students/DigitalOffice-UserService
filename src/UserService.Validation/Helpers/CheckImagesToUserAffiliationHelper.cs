using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Validation.Helpers.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Helpers
{
  public class CheckImagesToUserAffiliationHelper : ICheckImagesToUserAffiliationHelper
  {
    private readonly IImageRepository _imageRepository;

    public CheckImagesToUserAffiliationHelper(
      IImageRepository imageRepository)
    {
      _imageRepository = imageRepository;
    }

    public bool CheckAffiliation(List<Guid> imagesIds, Guid entityId)
    {
      List<DbEntityImage> dbEntityImages = _imageRepository.Get(imagesIds);

      foreach(DbEntityImage dbEntityImage in dbEntityImages)
      {
        if (dbEntityImage.EntityId != entityId)
        {
          return false;
        }
      }

      return true;
    }
  }
}
