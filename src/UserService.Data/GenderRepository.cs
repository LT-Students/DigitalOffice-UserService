using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System;
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
  }
}
