using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasIndex(r => r.Name).IsUnique();

            builder.Property(r => r.Created).HasDefaultValueSql("getdate()");

            builder.Property(r => r.Name).HasConversion(new Conversions.StringSetenceCaseFormatter());
            builder.Property(r => r.Description).HasConversion(new Conversions.StringSetenceCaseFormatter());

            builder.HasQueryFilter(r => r.Active);

            Permissions adminPermissions = Permissions.CreateUsers |
                Permissions.DeactivateUsers |
                Permissions.ReadRoles |
                Permissions.ReadUsers |
                Permissions.UpdateUsers |
                Permissions.WriteJobs;

            builder.HasData(
                new Role
                {
                    RoleId = 1,
                    Name = "Administrator",
                    Description = "User with administrative rights",
                    Permissions = adminPermissions,
                    Active = true
                });
        }
    }
}
