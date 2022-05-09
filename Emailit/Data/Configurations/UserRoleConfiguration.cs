using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            //ManyToMany User y Rol
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.HasData(
                new UserRole
                {
                    UserId = 1,
                    RoleId = 1
                });
        }
    }
}
