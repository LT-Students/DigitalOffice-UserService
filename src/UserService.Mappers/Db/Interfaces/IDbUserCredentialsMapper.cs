using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserCredentialsMapper
  {
    DbUserCredentials Map(
      CreateCredentialsRequest request,
      string salt,
      string passwordHash);
  }
}
