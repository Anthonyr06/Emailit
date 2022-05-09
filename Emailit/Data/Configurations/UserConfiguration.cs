using Emailit.Data;
using Emailit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly IConfiguration _Configuration;

        public UserConfiguration(IConfiguration configuration)
        {
            _Configuration = configuration;

        }
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasIndex(c => c.IdCard).IsUnique(); 

            builder.HasIndex(e => e.Email).IsUnique(); 

            builder.Property(u => u.Created).HasDefaultValueSql("getdate()");

            builder.Property(u => u.LoginAttempts).HasDefaultValue(0);

            builder.Property(u => u.Name).HasConversion(new Conversions.StringTitleCaseFormatter());
            builder.Property(u => u.Lastname).HasConversion(new Conversions.StringTitleCaseFormatter());
            builder.Property(u => u.Email).HasConversion(new Conversions.StringTitleCaseFormatterEmail());
            builder.Property(u => u.IdCard).HasConversion(new Conversions.IdCardFormatter());

            builder.HasQueryFilter(u => u.Active);

            string emailEmailitApp = _Configuration.GetValue<string>("EmailConfiguration:From").ToUpper();
            string idCardPowerUser = "00000000000";
            Permissions allPermissions = new Permissions();

            foreach (var permiso in Enum.GetValues(typeof(Permissions)))
            {
                allPermissions += (ulong)permiso;
            }

            var passwordHasher = new PasswordHasher<string>();

            builder.HasData(
                new User()
                {
                    UserId = 1,
                    IdCard = idCardPowerUser,
                    Name = "Patricia",
                    Lastname = "Moore",
                    Gender = Gender.other,
                    Email = emailEmailitApp,
                    Password = passwordHasher.HashPassword(null, idCardPowerUser),
                    Active = true,
                    MustChangePassword = true,
                    LoginAttempts = 0,
                    Permission = allPermissions
                });
        }
    }
}
