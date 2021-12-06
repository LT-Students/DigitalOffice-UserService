using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Data.Interfaces;
using LT.DigitalOffice.UserService.Models.Db;
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

    public async Task<Guid?> AddAsync(DbGender gender)
    {
      _provider.Genders.Add(gender);
      await _provider.SaveAsync();

      return gender.Id;
    }

    public bool DoesGenderAlreadyExist(string genderName)
    {
      return _provider.Genders.Any(s => s.Name == genderName);
    }
  }
}
