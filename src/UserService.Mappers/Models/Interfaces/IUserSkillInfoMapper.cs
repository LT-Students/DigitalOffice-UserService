using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IUserSkillInfoMapper
  {
    UserSkillInfo Map(UserSkillData userSkillData);
  }
}
