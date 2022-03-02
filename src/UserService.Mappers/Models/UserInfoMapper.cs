using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Enums;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class UserInfoMapper : IUserInfoMapper
  {
    public UserInfo Map(
      DbUser dbUser,
      ImageInfo avatar)
    {
      if (dbUser is null)
      {
        return null;
      }

      return new UserInfo
      {
        Id = dbUser.Id,
        FirstName = dbUser.FirstName,
        LastName = dbUser.LastName,
        MiddleName = dbUser.MiddleName,
        Status = (UserStatus)dbUser.Status,
        IsAdmin = dbUser.IsAdmin,
        IsActive = dbUser.IsActive,
        Avatar = avatar,
        Communications = dbUser.Communications
          ?.Select(c => new CommunicationInfo
          {
            Id = c.Id,
            Type = (CommunicationType)c.Type,
            Value = c.Value,
            IsConfirmed = c.IsConfirmed
          }),
      };
    }
  }
}
