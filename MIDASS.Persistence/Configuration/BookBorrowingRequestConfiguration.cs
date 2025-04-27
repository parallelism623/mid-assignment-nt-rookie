
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Configuration;

public class BookBorrowingRequestConfiguration : IEntityTypeConfiguration<BookBorrowingRequest>
{
    public void Configure(EntityTypeBuilder<BookBorrowingRequest> builder)
    {
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.HasOne(bb => bb.Approver)
            .WithMany(u => u.BookBorrowingApproves)
            .HasForeignKey(bb => bb.ApproverId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(bb => bb.Requester)
            .WithMany(u => u.BookBorrowingRequests)
            .HasForeignKey(bb => bb.RequesterId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(b => b.RequesterId);
    }
}
