
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASS.Domain.Entities;

namespace MIDASS.Persistence.Configuration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasIndex(u => u.RoleId);
        builder.HasQueryFilter(b => !b.IsDeleted);
        builder.Property(u => u.BookBorrowingLimit)
            .HasDefaultValue(3);
        builder.Property(x => x.Email).HasMaxLength(100);
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Username).HasMaxLength(32);
        builder.Property(x => x.Password).HasMaxLength(500);
    }
}
