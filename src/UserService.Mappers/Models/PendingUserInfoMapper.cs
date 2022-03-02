using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class PendingUserInfoMapper : IPendingUserInfoMapper
  {
    private readonly IUserInfoMapper _userInfoMapper;

    public PendingUserInfoMapper(
      IUserInfoMapper userInfoMapper)
    {
      _userInfoMapper = userInfoMapper;
    }

    public PendingUserInfo Map(
      DbUser dbUser,
      Guid invintationCommunicationId,
      ImageInfo avatar)
    {
      if (dbUser is null)
      {
        return default;
      }

      return new PendingUserInfo
      {
        User = _userInfoMapper.Map(dbUser, avatar),
        InvintationCommunicationId = invintationCommunicationId
      };
    }
  }
}
