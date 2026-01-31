using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Infrastructure.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(s => s.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(s => s.LastName).IsRequired().HasMaxLength(50);
        
        builder.Property(s => s.PersonalEmail).IsRequired().HasMaxLength(150);
        builder.HasIndex(s => s.PersonalEmail).IsUnique();

        builder.Property(s => s.Phone).HasMaxLength(20);

        builder.HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.ApplicationUser)
            .WithOne()
            .HasForeignKey<Student>(s => s.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade); 
            
        builder.HasIndex(s => s.ApplicationUserId).IsUnique();
    }
}