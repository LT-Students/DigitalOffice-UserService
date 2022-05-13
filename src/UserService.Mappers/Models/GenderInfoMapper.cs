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
      return dbGenders is null
        ? default
        : dbGenders.Select(x => new GenderInfo 
          {
            Id = x.Id,
            Name = x.Name
          }).ToList();
    }
  }
}
