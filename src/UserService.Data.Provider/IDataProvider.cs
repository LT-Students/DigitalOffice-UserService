using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.EFSupport.Provider;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CompanyService.Data.Provider
{
  [AutoInject(InjectType.Scoped)]
  public interface IDataProvider : IBaseDataProvider
  {
    DbSet<DbUser> Users { get; set; }
    DbSet<DbUserAddition> UsersAdditions { get; set; }
    DbSet<DbUserCredentials> UsersCredentials { get; set; }
    DbSet<DbUserCommunication> UsersCommunications { get; set; }
    DbSet<DbPendingUser> PendingUsers { get; set; }
    DbSet<DbUserAvatar> UsersAvatars { get; set; }
    DbSet<DbGender> Genders { get; set; }
  }
}