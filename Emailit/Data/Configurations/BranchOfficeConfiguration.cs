using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class BranchOfficeConfiguration : IEntityTypeConfiguration<BranchOffice>
    {
        public void Configure(EntityTypeBuilder<BranchOffice> builder)
        {
            builder.HasIndex(e => e.Name).IsUnique();
            builder.HasIndex(e => e.ManagerId).IsUnique();

            builder.Property(e => e.Name).HasConversion(new Conversions.StringTitleCaseFormatter());

            builder.HasQueryFilter(e => e.Active);
        }
    }
}
