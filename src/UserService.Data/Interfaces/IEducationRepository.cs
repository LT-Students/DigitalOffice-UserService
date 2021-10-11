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
        Task Add(DbUserEducation education);

        DbUserEducation Get(Guid educationId);

        Task<bool> Edit(DbUserEducation educationId, JsonPatchDocument<DbUserEducation> request);

        Task<bool> Remove(DbUserEducation education);
    }
}
