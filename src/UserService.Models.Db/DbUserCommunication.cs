using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbUserCommunication
    {
        public const string TableName = "UserCommunications";

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedAtUtc { get; set; }
        public DbUser User { get; set; }
    }

    public class DbConnectionConfiguration : IEntityTypeConfiguration<DbUserCommunication>
    {
        public void Configure(EntityTypeBuilder<DbUserCommunication> builder)
        {
            builder
                .ToTable(DbUserCommunication.TableName);

            builder
                .HasKey(uc => uc.Id);

            builder
                .HasOne(uc => uc.User)
                .WithMany(u => u.Communications);
        }
    }
}
