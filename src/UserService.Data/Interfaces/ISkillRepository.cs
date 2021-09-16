using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using System;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    [AutoInject]
    public interface ISkillRepository
    {
        Guid Add(DbSkill skill);
        bool DoesSkillAlreadyExist(string skillName);
    }
}
