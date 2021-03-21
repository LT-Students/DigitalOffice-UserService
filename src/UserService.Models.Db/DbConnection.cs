using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            builder.HasKey(id => new {id.UserId, id.Id});

            builder.HasOne(us => us.User)
                .WithMany(con => con.Connections)
                .HasForeignKey(id => id.UserId);
        }
    }
}
