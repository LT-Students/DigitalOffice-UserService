using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
    /// <summary>
    /// Represents mapper. Provides methods for converting an object of user value <see cref="CreateCredentialsRequest"/>
    /// type into an object of <see cref="DbUserCredentials"/> type according to some rule.
    /// </summary>
    [AutoInject]
    public interface IDbUserCredentialsMapper
    {
        DbUserCredentials Map(
            CreateCredentialsRequest request,
            string salt,
            string passwordHash);
    }
}
