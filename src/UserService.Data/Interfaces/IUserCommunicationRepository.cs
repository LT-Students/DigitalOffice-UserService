using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IUserCommunicationRepository
  {
    Task<Guid?> CreateAsync(DbUserCommunication dbUserCommunication);

    Task<bool> EditAsync(Guid communicationId, string newValue);

    Task<bool> Confirm(Guid communicationId);

    Task<DbUserCommunication> GetAsync(Guid communicationId);

    Task<DbUserCommunication> GetBaseAsync(Guid userId);

    Task SetBaseTypeAsync(Guid communicationId, Guid modifiedBy);

    Task RemoveBaseTypeAsync(Guid userId);

    Task<bool> RemoveAsync(DbUserCommunication dbUserCommunication);

    Task<bool> DoesValueExist(string value);
  }
}
