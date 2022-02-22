using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbPendingUser
  {
    public const string TableName = "PendingUsers";

    public Guid UserId { get; set; }
    public string Password { get; set; }
    public DbUser User { get; set; }
  }

  public class DbPendingUserConfiguration : IEntityTypeConfiguration<DbPendingUser>
  {
    public void Configure(EntityTypeBuilder<DbPendingUser> builder)
    {
      builder
        .ToTable(DbPendingUser.TableName);

      builder
        .HasKey(pu => pu.UserId);

      builder
        .Property(pu => pu.Password)
        .IsRequired();

      builder
        .HasOne(pu => pu.User)
        .WithOne(uc => uc.Pending);
    }
  }
}
