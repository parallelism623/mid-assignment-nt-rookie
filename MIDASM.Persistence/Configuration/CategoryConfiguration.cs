
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASM.Domain.Constrants.Validations;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.Property(c => c.Name).HasMaxLength(CategoryValidationRules.MaxLengthCategoryName);
        builder.Property(c => c.Description).HasMaxLength(CategoryValidationRules.MaxLengthCategoryDescription);
    }
}
