using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.HasIndex(pu => pu.Name).IsUnique();

            builder.Property(pu => pu.Name).HasConversion(new Conversions.StringSetenceCaseFormatter());
            builder.Property(pu => pu.Description).HasConversion(new Conversions.StringSetenceCaseFormatter());

            builder.HasQueryFilter(r => r.Active);

        }
    }
}
