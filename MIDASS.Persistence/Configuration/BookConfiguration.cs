
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASS.Domain.Entities;
using System.Reflection.Emit;

namespace MIDASS.Persistence.Configuration;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.Property(b => b.Title)
            .HasMaxLength(100);
        builder.Property(b => b.Description)
            .HasMaxLength(2000);
        builder.Property(b => b.Author)
            .HasMaxLength(100);
        builder.HasIndex(b => b.CategoryId);
    }
}
