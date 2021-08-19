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


        public void Add(DbSkill skill)
        {
            if (skill == null)
            {
                throw new ArgumentNullException(nameof(skill));
            }

            _provider.Skills.Add(skill);
            _provider.Save();
        }

        public bool DoesSkillAlreadyExist(string skillName)
        {
            if (string.IsNullOrEmpty(skillName))
            {
                throw new ArgumentException(nameof(skillName));
            }

            return _provider.Skills.Any(s => s.Name == skillName);
        }
    }
}
