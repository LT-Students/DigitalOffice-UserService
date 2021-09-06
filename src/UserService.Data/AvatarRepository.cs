using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
  public class AvatarRepository : IAvatarRepository
  {
    private readonly IDataProvider _provider;

    public AvatarRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public List<Guid> Create(List<DbUserAvatar> dbUserAvatars)
    {
      if (dbUserAvatars == null || dbUserAvatars.Contains(null))
      {
        return null;
      }

      _provider.UsersAvatars.AddRange(dbUserAvatars);
      _provider.Save();

      return dbUserAvatars.Select(x => x.ImageId).ToList();
    }

    public List<DbUserAvatar> Get(Guid userId)
    {
      return _provider.UsersAvatars.Where(x => x.UserId == userId).ToList();
    }

    public List<DbUserAvatar> Get(List<Guid> imagesIds)
    {
      if (imagesIds == null)
      {
        return null;
      }

      return _provider.UsersAvatars.Where(x => imagesIds.Contains(x.ImageId)).ToList();
    }

    public bool Remove(List<Guid> imagesIds)
    {
      if (imagesIds == null)
      {
        return false;
      }

      List<DbUserAvatar> removeUsersAvatars = Get(imagesIds);

      _provider.UsersAvatars.RemoveRange(removeUsersAvatars);
      _provider.Save();

      return true;
    }
  }
}
