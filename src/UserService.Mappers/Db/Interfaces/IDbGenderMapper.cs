using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.User;

namespace LT.DigitalOffice.UserService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbGenderMapper
  {
    DbGender Map(CreateGenderRequest request);
  }
}
