using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  /// <summary>
  /// Represents interface of repository in repository pattern.
  /// Provides methods for working with the database of UserService.
  /// </summary>
  [AutoInject]
  public interface IUserCredentialsRepository
  {
    /// <summary>
    /// Returns the user credentials.
    /// </summary>
    DbUserCredentials Get(GetCredentialsFilter filter);

    Task<Guid> CreateAsync(DbUserCredentials dbUserCredentials);

    Task SwitchActiveStatusAsync(Guid userId, bool isActiveStatus);

    Task<bool> EditAsync(DbUserCredentials userCredentials);

    bool IsLoginExist(string login);

    bool IsCredentialsExist(Guid userId);
  }
}
