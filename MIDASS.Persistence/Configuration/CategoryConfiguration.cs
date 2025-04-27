
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.Property(c => c.Name).HasMaxLength(100);
        builder.Property(c => c.Description).HasMaxLength(2000);
    }
}
