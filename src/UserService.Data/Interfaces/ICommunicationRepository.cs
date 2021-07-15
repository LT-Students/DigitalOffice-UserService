using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    [AutoInject]
    public interface ICommunicationRepository
    {
        DbUserCommunication Get(Guid communicationId);

        Guid Add(DbUserCommunication userCommunication);

        bool Edit(Guid communicationId, JsonPatchDocument<DbUserCommunication> request);

        bool Remove(DbUserCommunication communication);

        bool IsCommunicationValueExist(string value);
    }
}
