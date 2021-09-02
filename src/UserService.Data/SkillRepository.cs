using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using System;
using System.Linq;

namespace LT.DigitalOffice.UserService.Data
{
    public class SkillRepository : ISkillRepository
    {
        private readonly IDataProvider _provider;

        public SkillRepository(IDataProvider provider)
        {
            _provider = provider;
        }

        public Guid Add(DbSkill skill)
        {
            if (skill == null)
            {
                return default;
            }

            _provider.Skills.Add(skill);
            _provider.Save();

            return skill.Id;
        }

        public bool DoesSkillAlreadyExist(string skillName)
        {
            return _provider.Skills.Any(s => s.Name == skillName);
        }
    }
}
