using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef
{
    /// <summary>
    /// A class that defines the tables and its properties in the database.
    /// For this particular case, it defines the database for the UserService.
    /// </summary>
    public class UserServiceDbContext : DbContext, IDataProvider
    {
        public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbAchievement> Achievements { get; set; }

        // Fluent API is written here.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("LT.DigitalOffice.UserService.Models.Db"));
        }

        public object MakeEntityDetached(object obj)
        {
            this.Entry(obj).State = EntityState.Detached;
            return this.Entry(obj).State;
        }

        void IDataProvider.Save()
        {
            this.SaveChanges();
        }

        public void EnsureDeleted()
        {
            this.Database.EnsureDeleted();
        }

        public bool IsInMemory()
        {
            return this.Database.IsInMemory();
        }
    }
}