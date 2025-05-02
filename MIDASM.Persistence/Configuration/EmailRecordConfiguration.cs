
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Configuration;

public class EmailRecordConfiguration : IEntityTypeConfiguration<EmailRecord>
{
    public void Configure(EntityTypeBuilder<EmailRecord> builder)
    {
        builder.Property(e => e.AttachFile)
            .IsRequired()                    
            .HasColumnType("varbinary(max)");
    }
}
