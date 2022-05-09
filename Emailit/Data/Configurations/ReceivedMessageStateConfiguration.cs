using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class ReceivedMessageStateConfiguration : IEntityTypeConfiguration<ReceivedMessageState>
    {
        public void Configure(EntityTypeBuilder<ReceivedMessageState> builder)
        {
            builder.HasOne(em => em.ReceivedMessage).WithMany(mr => mr.States).HasForeignKey(em => new { em.UserId, em.MessageId }).OnDelete(DeleteBehavior.Restrict);

            builder.Property(em => em.Date).HasDefaultValueSql("getdate()");
        }
    }
}
