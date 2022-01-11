using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class GenderInfoMapper : IGenderInfoMapper
  {
    public List<GenderInfo> Map(List<DbGender> dbGender)
    {
      if (dbGender is null)
      {
        return null;
      }

      return dbGender.Select(x => new GenderInfo { Name = x.Name }).ToList();
    }
  }
}
