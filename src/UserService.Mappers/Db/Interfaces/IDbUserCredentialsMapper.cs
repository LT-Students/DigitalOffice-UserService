using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;
using System;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserCredentialsMapper
  {
    DbUserCredentials Map(CreateCredentialsRequest request);

    DbUserCredentials Map(
      Guid userId,
      string login,
      string password);
  }
}
