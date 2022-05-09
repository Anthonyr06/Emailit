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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.Property(m => m.Date).HasDefaultValueSql("getdate()");

            builder.Property(m => m.Tittle).HasConversion(new Conversions.StringSetenceCaseFormatter());
            builder.Property(m => m.Body).HasConversion(new Conversions.HtmlToString());
            builder.Property(m => m.BodyInHtml).HasConversion(new Conversions.HtmlSanitizer());
        }
    }
}
