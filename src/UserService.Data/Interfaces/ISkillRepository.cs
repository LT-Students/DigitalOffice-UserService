using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface ISkillRepository
  {
    Task<Guid> AddAsync(DbSkill skill);

    bool DoesSkillAlreadyExist(string skillName);
  }
}
