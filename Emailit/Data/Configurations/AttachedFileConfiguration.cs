using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class AttachedFileConfiguration : IEntityTypeConfiguration<AttachedFile>
    {
        public void Configure(EntityTypeBuilder<AttachedFile> builder)
        {
            //ManyToMany Message y FileData
            builder.HasKey(ur => new { ur.MessageId, ur.FileId });

        }
    }
}

