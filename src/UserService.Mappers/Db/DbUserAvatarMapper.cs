using LT.DigitalOffice.UserService.Mappers.Db.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Mappers.Db
{
  public class DbUserAvatarMapper : IDbUserAvatarMapper
  {
    public List<DbUserAvatar> Map(List<Guid> imageIds, Guid userId)
    {
      List<DbUserAvatar> result = new();

      if (imageIds == null)
      {
        return result;
      }

      foreach (Guid imageId in imageIds)
      {
        result.Add(new DbUserAvatar
        {
          Id = Guid.NewGuid(),
          ImageId = imageId,
          UserId = userId
        });
      }

      return result;
    }
  }
}
