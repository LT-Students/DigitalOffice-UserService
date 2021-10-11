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
        DbUserCommunication Get(Guid communicationId);

        Task<Guid> Add(DbUserCommunication userCommunication);

        Task<bool> Edit(Guid communicationId, JsonPatchDocument<DbUserCommunication> request);

        Task<bool> Remove(DbUserCommunication communication);

        bool IsCommunicationValueExist(string value);
    }
}
