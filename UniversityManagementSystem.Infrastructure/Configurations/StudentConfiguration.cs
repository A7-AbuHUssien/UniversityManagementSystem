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
        
        builder.Property(s => s.Email).IsRequired().HasMaxLength(150);
        builder.HasIndex(s => s.Email).IsUnique();

        builder.Property(s => s.Phone).HasMaxLength(20);

        builder.HasOne(s => s.Department)
            .WithMany(d => d.Students)
            .HasForeignKey(s => s.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}