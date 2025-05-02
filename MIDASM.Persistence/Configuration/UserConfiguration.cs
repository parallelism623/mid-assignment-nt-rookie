
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MIDASM.Contract.Messages.Validations;
using MIDASM.Domain.Constrants.Validations;
using MIDASM.Domain.Entities;

namespace MIDASM.Persistence.Configuration;

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
        builder.Property(x => x.Email).HasMaxLength(UserValidationRules.MaxLengthEmail);
        builder.Property(x => x.FirstName).HasMaxLength(UserValidationRules.MaxLengthFirstName);
        builder.Property(x => x.LastName).HasMaxLength(UserValidationRules.MaxLengthLastName);
        builder.Property(x => x.PhoneNumber).HasMaxLength(UserValidationRules.MaxLengthPhoneNumber);
        builder.Property(x => x.Username).HasMaxLength(UserValidationRules.MaxLengthUsername);
        builder.Property(x => x.Password).HasMaxLength(UserValidationRules.MaxLengthHashPassword);
    }
}
