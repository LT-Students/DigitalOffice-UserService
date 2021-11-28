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
    DbSet<DbSkill> Skills { get; set; }
    DbSet<DbUserCredentials> UserCredentials { get; set; }
    DbSet<DbUserSkill> UserSkills { get; set; }
    DbSet<DbUserAchievement> UserAchievements { get; set; }
    DbSet<DbUserCertificate> UserCertificates { get; set; }
    DbSet<DbUserEducation> UserEducations { get; set; }
    DbSet<DbUserCommunication> UserCommunications { get; set; }
    DbSet<DbPendingUser> PendingUsers { get; set; }
    DbSet<DbEntityImage> EntitiesImages { get; set; }
    DbSet<DbUserLocation> UsersLocations { get; set; }
    DbSet<DbGender> Genders { get; set; }
    DbSet<DbUserGender> UsersGenders { get; set; }
  }
}