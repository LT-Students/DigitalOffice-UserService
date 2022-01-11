using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using LT.DigitalOffice.UserService.Models.Dto.Requests.Gender.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data
{
  public class GenderRepository : IGenderRepository
  {
    private readonly IDataProvider _provider;

    public GenderRepository(IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<Guid?> CreateAsync(DbGender gender)
    {
      _provider.Genders.Add(gender);
      await _provider.SaveAsync();

      return gender.Id;
    }

    public async Task<bool> DoesGenderAlreadyExistAsync(string genderName)
    {
      return await _provider.Genders.AnyAsync(s => s.Name.ToLower() == genderName.ToLower());
    }

    public async Task<(List<DbGender> dbGenders, int totalCount)> FindGendersAsync(FindGendersFilter filter)
    {
      if (filter == null)
      {
        return (null, default);
      }

      IQueryable<DbGender> dbGenders = _provider.Genders.Where(g => g.Name
        .Contains(filter.Name)).AsQueryable();

      return ( 
        await dbGenders.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(),
        await dbGenders.CountAsync());
    }
  }
}
