using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CompanyService.Data.Provider
{
  [AutoInject(InjectType.Scoped)]
  public interface IDataProvider : IBaseDataProvider
  {
    DbSet<DbUser> Users { get; set; }
    DbSet<DbAchievement> Achievements { get; set; }
    DbSet<DbUserCredentials> UsersCredentials { get; set; }
    DbSet<DbUserAchievement> UserAchievements { get; set; }
    DbSet<DbUserCommunication> UserCommunications { get; set; }
    DbSet<DbPendingUser> PendingUsers { get; set; }
    DbSet<DbUserAvatar> UsersAvatars { get; set; }
    DbSet<DbGender> Genders { get; set; }
    DbSet<DbUserGender> UsersGenders { get; set; }
    DbSet<DbUserAddition> UsersAdditions { get; set; }
  }
}