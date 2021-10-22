using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface ICommunicationRepository
  {
    Task<DbUserCommunication> GetAsync(Guid communicationId);

    Task<Guid?> CreateAsync(DbUserCommunication dbUserCommunication);

    Task<bool> EditAsync(Guid communicationId, JsonPatchDocument<DbUserCommunication> request);

    Task<bool> RemoveAsync(DbUserCommunication dbUserCommunication);

    Task<bool> CheckExistingValue(string value);

    Task<int> CountUserEmails(Guid userId);
  }
}
