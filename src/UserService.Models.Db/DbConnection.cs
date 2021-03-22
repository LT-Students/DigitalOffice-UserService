using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace LT.DigitalOffice.UserService.Models.Db
{
    public class DbConnection
    {
        public const string TableName = "Connections";

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public DbUser User { get; set; }
    }

    public class DbConnectionConfiguration : IEntityTypeConfiguration<DbConnection>
    {
        public void Configure(EntityTypeBuilder<DbConnection> builder)
        {
            builder
                .ToTable(DbConnection.TableName);

            builder
                .HasKey(conn =>  conn.Id);

            builder
                .HasOne(conn => conn.User)
                .WithMany(u => u.Connections)
                .HasForeignKey(conn => conn.UserId);
        }
    }
}
