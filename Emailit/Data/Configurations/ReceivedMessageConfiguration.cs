using Emailit.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emailit.Data.Configurations
{
    public class ReceivedMessageConfiguration : IEntityTypeConfiguration<ReceivedMessage>
    {
        public void Configure(EntityTypeBuilder<ReceivedMessage> builder)
        {
            builder.HasKey(mr => new { mr.UserId, mr.MessageId });

        }
    }
}
