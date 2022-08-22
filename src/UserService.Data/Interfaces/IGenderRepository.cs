using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IGenderRepository
  {
    Task CreateAsync(DbGender gender);

    Task<bool> DoesGenderAlreadyExistAsync(string genderName);

    Task<(List<DbGender> dbGenders, int totalCount)> FindGendersAsync(FindGendersFilter filter);
  }
}
