using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserAvatar
  {
    public const string TableName = "UsersAvatars";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ImageId { get; set; }
    public DbUser User { get; set; }
  }

  public class DbUserAvatarConfiguration : IEntityTypeConfiguration<DbUserAvatar>
  {
    public void Configure(EntityTypeBuilder<DbUserAvatar> builder)
    {
      builder
          .ToTable(DbUserAvatar.TableName);

      builder
          .HasKey(c => c.Id);

      builder
          .HasOne(pm => pm.User)
          .WithMany(p => p.Avatars);
    }
  }
}
