using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    [AutoInject]
    public interface ICertificateRepository
    {
        void Add(DbUserCertificate certificate);

        DbUserCertificate Get(Guid certificateId);

        bool Edit(DbUserCertificate certificateId, JsonPatchDocument<DbUserCertificate> request);

        bool Remove(DbUserCertificate certificate);
    }
}
