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
            builder.ToTable("UserConnections");
            
            builder.HasKey(pm => new {pm.UserId, pm.Id});
            builder.HasOne(pm => pm.User)
                .WithMany(pm => pm.Connections)
                .HasForeignKey(pm => pm.UserId);
        }
    }
}
