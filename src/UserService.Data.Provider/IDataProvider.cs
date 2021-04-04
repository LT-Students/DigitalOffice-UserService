using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CompanyService.Data.Provider
{
    public interface IDataProvider : IBaseDataProvider
    {
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbAchievement> Achievements { get; set; }
        public DbSet<DbSkill> Skills { get; set; }
        public DbSet<DbUserCredentials> UserCredentials { get; set; }
        public DbSet<DbUserSkill> UserSkills { get; set; }
        public DbSet<DbUserAchievement> UserAchievements { get; set; }
        public DbSet<DbUserCertificate> UserCertificates { get; set; }
        public DbSet<DbUserCommunication> UserCommunications { get; set; }
        public DbSet<DbPendingUser> PendingUsers { get; set; }
    }
}