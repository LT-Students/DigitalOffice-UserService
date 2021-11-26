using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
  public class DbUserCredentials
  {
    public const string TableName = "UserCredentials";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public string Salt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
    public DbUser User { get; set; }
  }

  public class UserCredentialsConfiguration : IEntityTypeConfiguration<DbUserCredentials>
  {
    public void Configure(EntityTypeBuilder<DbUserCredentials> builder)
    {

      builder
        .ToTable(DbUserCredentials.TableName);

      builder
        .HasKey(uc => uc.Id);

      builder
        .Property(uc => uc.Login)
        .IsRequired();

      builder
        .Property(uc => uc.PasswordHash)
        .IsRequired();

      builder
        .Property(uc => uc.Salt)
        .IsRequired();

      builder
        .HasOne(uc => uc.User)
        .WithOne(u => u.Credentials);
    }
  }
}
