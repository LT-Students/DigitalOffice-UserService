using LT.DigitalOffice.UserService.Mappers.Models.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Models;
using System.Collections.Generic;
using System.Linq;

namespace LT.DigitalOffice.UserService.Mappers.Models
{
  public class GenderInfoMapper : IGenderInfoMapper
  {
    public List<GenderInfo> Map(List<DbGender> dbGenders)
    {
      if (dbGenders is null)
      {
        return null;
      }

      return dbGenders.Select(x => new GenderInfo { Name = x.Name }).ToList();
    }
  }
}
