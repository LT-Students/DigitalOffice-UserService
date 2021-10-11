using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    [AutoInject]
    public interface ICertificateRepository
    {
        Task AddAsync(DbUserCertificate certificate);

        DbUserCertificate Get(Guid certificateId);

        Task<bool> EditAsync(DbUserCertificate certificateId, JsonPatchDocument<DbUserCertificate> request);

        Task<bool> RemoveAsync(DbUserCertificate certificate);
    }
}
