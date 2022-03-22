using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Skill;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Broker.Requests.Interfaces
{
  [AutoInject]
  public interface ISkillService
  {
    Task<List<UserSkillData>> GetSkillsAsync(Guid userId, List<string> errors);
  }
}
