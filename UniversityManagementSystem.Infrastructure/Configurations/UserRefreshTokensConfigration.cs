using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Infrastructure.Configurations;

public class UserRefreshTokensConfigration : IEntityTypeConfiguration<UserRefreshToken>
{
  
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(500); 

        builder.Property(t => t.ExpiryDate)
            .IsRequired();

        builder.Property(t => t.IsRevoked)
            .HasDefaultValue(false);

        builder.HasOne(t => t.User)
            .WithMany() 
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Token)
            .IsUnique();
    }
}