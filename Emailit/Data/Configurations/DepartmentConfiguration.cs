using Emailit.Data;
using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emailit.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasIndex(d => new { d.Name, d.BranchOfficeId }).IsUnique(); //only one department name per BranchOffice
            builder.HasIndex(d => d.ManagerId).IsUnique();

            builder.Property(d => d.Name).HasConversion(new Conversions.StringSetenceCaseFormatter());

            builder.HasQueryFilter(d => d.Active);
        }
    }
}
