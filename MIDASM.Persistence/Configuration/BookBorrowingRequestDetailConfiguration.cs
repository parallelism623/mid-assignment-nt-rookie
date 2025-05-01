
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASM.Domain.Constrants;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Configuration;

public class BookBorrowingRequestDetailConfiguration : IEntityTypeConfiguration<BookBorrowingRequestDetail>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequestDetail> builder)
    {
        builder.HasOne(bb => bb.Book)
            .WithMany(b => b.BookBorrowingRequestDetails)
            .HasForeignKey(bb => bb.BookId);

        builder.HasOne(bb => bb.BookBorrowingRequest)
            .WithMany(b => b.BookBorrowingRequestDetails)
            .HasForeignKey(bb => bb.BookBorrowingRequestId);
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.Property(bd => bd.Noted).HasMaxLength(BookBorrowingRequestDetailValidationRules.MaxLengthNoted);
        builder.HasIndex(bd => bd.BookBorrowingRequestId);
        builder.HasIndex(bd => bd.BookId);

    }
}
