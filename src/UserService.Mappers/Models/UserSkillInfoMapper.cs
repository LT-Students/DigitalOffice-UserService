using LT.DigitalOffice.Models.Broker.Models.Skill;
using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Dto.Models;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class UserSkillInfoMapper : IUserSkillInfoMapper
  {
    public SkillInfo Map(UserSkillData userSkillData)
    {
      if (userSkillData is null)
      {
        return null;
      }

      return new SkillInfo
      {
        Id = userSkillData.Id,
        Name = userSkillData.Name,
      };
    }
  }
}
