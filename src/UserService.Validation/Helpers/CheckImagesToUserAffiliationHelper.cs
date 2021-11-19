using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Validation.Helpers.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

    public async Task<bool> CheckAffiliationAsync(List<Guid> imagesIds, Guid entityId)
    {
      List<DbUserAvatar> dbEntityImages = await _imageRepository.GetAsync(imagesIds);

      foreach(DbUserAvatar dbEntityImage in dbEntityImages)
      {
        if (dbEntityImage.UserId != entityId)
        {
          return false;
        }
      }

      return true;
    }
  }
}
