
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASM.Domain.Constrants;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Configuration;

public class BookReviewConfiguration : IEntityTypeConfiguration<BookReview>
{
    public void Configure(EntityTypeBuilder<BookReview> builder)
    {
        builder.HasIndex(b => b.BookId);
        builder.HasIndex(b => b.ReviewerId);
        builder.Property(b => b.Title).HasMaxLength(BookReviewValidationRules.TitleMaxLength);
        builder.Property(b => b.Content).HasMaxLength(BookReviewValidationRules.ContentMaxLength);

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
