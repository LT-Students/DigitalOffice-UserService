using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
    [AutoInject]
    public interface ISkillRepository
    {
        void Add(DbSkill skill);
        bool DoesSkillAlreadyExist(string skillName);
    }
}
