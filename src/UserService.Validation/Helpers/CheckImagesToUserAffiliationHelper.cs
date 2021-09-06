using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Validation.Helpers.Interfaces;
using System;
using System.Collections.Generic;

namespace LT.DigitalOffice.UserService.Validation.Helpers
{
  public class CheckImagesToUserAffiliationHelper : ICheckImagesToUserAffiliationHelper
  {
    private readonly IAvatarRepository _avatarRepository;

    public CheckImagesToUserAffiliationHelper(
      IAvatarRepository avatarRepository)
    {
      _avatarRepository = avatarRepository;
    }

    public bool CheckAffiliation(List<Guid> imagesIds, Guid userId)
    {
      List<DbUserAvatar> dbUsersAvatars = _avatarRepository.Get(imagesIds);

      foreach(DbUserAvatar dbUserAvatar in dbUsersAvatars)
      {
        if (dbUserAvatar.UserId != userId)
        {
          return false;
        }
      }

      return true;
    }
  }
}
