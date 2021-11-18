using LT.DigitalOffice.CompanyService.Data.Provider;
using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.UserService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.Tasks;

namespace LT.DigitalOffice.UserService.Data.Provider.MsSql.Ef
{
  /// <summary>
  /// A class that defines the tables and its properties in the database.
  /// For this particular case, it defines the database for the UserService.
  /// </summary>
  public class UserServiceDbContext : DbContext, IDataProvider
  {
    public DbSet<DbUser> Users { get; set; }
    public DbSet<DbAchievement> Achievements { get; set; }
    public DbSet<DbSkill> Skills { get; set; }
    public DbSet<DbUserCredentials> UserCredentials { get; set; }
    public DbSet<DbUserSkill> UserSkills { get; set; }
    public DbSet<DbUserAchievement> UserAchievements { get; set; }
    public DbSet<DbUserCertificate> UserCertificates { get; set; }
    public DbSet<DbUserCommunication> UserCommunications { get; set; }
    public DbSet<DbUserEducation> UserEducations { get; set; }
    public DbSet<DbPendingUser> PendingUsers { get; set; }
    public DbSet<DbEntityImage> EntitiesImages { get; set; }
    public DbSet<DbUserLocation> UsersLocations { get; set; }
    public DbSet<DbUserGender> UserGenders { get; set; }
    public DbSet<DbGender> Genders { get; set; }

    public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options)
      : base(options)
    {
    }

    // Fluent API is written here.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("LT.DigitalOffice.UserService.Models.Db"));
    }

    public object MakeEntityDetached(object obj)
    {
      Entry(obj).State = EntityState.Detached;
      return Entry(obj).State;
    }

    async Task IBaseDataProvider.SaveAsync()
    {
      await SaveChangesAsync();
    }

    void IBaseDataProvider.Save()
    {
      SaveChanges();
    }

    public void EnsureDeleted()
    {
      Database.EnsureDeleted();
    }

    public bool IsInMemory()
    {
      return Database.IsInMemory();
    }
  }
}