using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Credentials.Filters;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IUserCredentialsRepository
  {
    Task<DbUserCredentials> GetAsync(GetCredentialsFilter filter);

    Task<Guid?> CreateAsync(DbUserCredentials dbUserCredentials);

    Task<bool> EditAsync(DbUserCredentials userCredentials);

    Task<bool> DoesLoginExistAsync(string login);

    Task<bool> DoesExistAsync(Guid userId);
  }
}
