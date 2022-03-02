using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IPendingUserInfoMapper
  {
    public PendingUserInfo Map(
      DbUser dbUser,
      Guid invintationCommunicationId,
      ImageInfo avatar);
  }
}
