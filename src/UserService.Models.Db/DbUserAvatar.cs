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
    public bool IsCurrentAvatar { get; set; }

    public DbUser User {get; set; }
  }

  public class DbEntityImageConfiguration : IEntityTypeConfiguration<DbUserAvatar>
  {
    public void Configure(EntityTypeBuilder<DbUserAvatar> builder)
    {
      builder
        .ToTable(DbUserAvatar.TableName);

      builder
        .HasKey(ua => ua.Id);

      builder
        .HasOne(ua => ua.User)
        .WithMany(u => u.Avatars);
    }
  }
}
