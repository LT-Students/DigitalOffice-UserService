using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Interfaces
{
  [AutoInject]
  public interface IGenderRepository
  {
    Task<Guid?> CreateAsync(DbGender gender);

    Task<bool> DoesGenderAlreadyExistAsync(string genderName);

    Task<(List<DbGender> dbGender, int totalCount)> FindGendersAsync(FindGendersFilter filter);
  }
}
