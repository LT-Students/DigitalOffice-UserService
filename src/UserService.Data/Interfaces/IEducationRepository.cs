using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IEducationRepository
  {
    void Add(DbUserEducation education);

    DbUserEducation Get(Guid educationId);

    bool Edit(DbUserEducation educationId, JsonPatchDocument<DbUserEducation> request);

    bool Remove(DbUserEducation education);
  }
}
