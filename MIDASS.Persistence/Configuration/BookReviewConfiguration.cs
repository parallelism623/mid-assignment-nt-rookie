
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Configuration;

public class BookReviewConfiguration : IEntityTypeConfiguration<BookReview>
{
    public void Configure(EntityTypeBuilder<BookReview> builder)
    {
        builder.HasKey(b => b.BookId);
        builder.HasKey(b => b.ReviewerId);
        builder.Property(b => b.Title).HasMaxLength(255);
        builder.Property(b => b.Content).HasMaxLength(2000);

        builder.HasQueryFilter(b => !b.IsDeleted);

        builder.HasOne(br => br.Reviewer)
            .WithMany(u => u.BookReviews)
            .HasForeignKey(u => u.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(br => br.Book)
            .WithMany(b => b.BookReviews)
            .HasForeignKey(br => br.BookId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
