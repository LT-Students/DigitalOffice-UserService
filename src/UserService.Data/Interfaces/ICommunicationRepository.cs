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
    Task<Guid?> CreateAsync(DbUserCommunication dbUserCommunication);

    Task<bool> EditAsync(
      DbUserCommunication dbUserCommunication,
      JsonPatchDocument<DbUserCommunication> request);

    Task<bool> Confirm(Guid communicationId);

    Task<DbUserCommunication> GetAsync(Guid communicationId);

    Task ActivateFirstCommunicationAsync(Guid userId);

    Task RemoveBaseTypeAsync(Guid userId);

    Task<bool> RemoveAsync(DbUserCommunication dbUserCommunication);

    Task<bool> DoesValueExist(string value);
  }
}
