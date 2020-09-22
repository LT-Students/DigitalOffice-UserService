using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.CompanyService.Data.Provider
{
    public interface IDataProvider
    {
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbAchievement> Achievements { get; set; }

        void Save();
        object MakeEntityDetached(object obj);
        void EnsureDeleted();
        bool IsInMemory();
    }
}