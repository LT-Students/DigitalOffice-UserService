using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    [AutoInject]
    public interface IEducationRepository
    {
        Task AddAsync(DbUserEducation education);

        DbUserEducation Get(Guid educationId);

        Task<bool> EditAsync(DbUserEducation educationId, JsonPatchDocument<DbUserEducation> request);

        Task<bool> RemoveAsync(DbUserEducation education);
    }
}
