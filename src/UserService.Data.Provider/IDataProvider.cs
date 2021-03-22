using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CompanyService.Data.Provider
{
    public interface IDataProvider : IBaseDataProvider
    {
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbUserCredentials> UserCredentials { get; set; }
        public DbSet<DbAchievement> Achievements { get; set; }
        public DbSet<DbUserSkills> UserSkills { get; set; }
        public DbSet<DbSkill> Skills { get; set; }
        public DbSet<DbConnection> Connections { get; set; }
    }
}