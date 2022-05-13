using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class UserSkillInfoMapper : IUserSkillInfoMapper
  {
    public SkillInfo Map(UserSkillData userSkillData)
    {
      return userSkillData is null
        ? default
        : new SkillInfo
        {
          Id = userSkillData.Id,
          Name = userSkillData.Name,
        };
    }
  }
}
