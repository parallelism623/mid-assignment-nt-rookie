
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASM.Domain.Constrants.Validations;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.HasIndex(b => b.CategoryId);
        builder.Property(b => b.Title)
            .HasMaxLength(BookValidationRules.MaxLengthTitle);
        builder.Property(b => b.Description)
            .HasMaxLength(BookValidationRules.MaxLengthDescription);
        builder.Property(b => b.Author)
            .HasMaxLength(BookValidationRules.MaxLengthAuthor);
        builder.HasIndex(b => b.CategoryId);

        builder.Property(o => o.SubImagesUrl)
            .HasConversion(
                v => v == null
                    ? null
                    : string.Join(";-;", v),
                v => string.IsNullOrEmpty(v)
                    ? new List<string>()
                    : v.Split(new[] { ";-;" }, StringSplitOptions.None)
                .ToList()
            ); 

    }
}
