using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Infrastructure.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.Property(c => c.Title).IsRequired().HasMaxLength(200);
        
        builder.Property(c => c.CourseCode).IsRequired().HasMaxLength(15);
        builder.HasIndex(c => c.CourseCode).IsUnique();

        builder.Property(c => c.Credits).HasDefaultValue(3);

        builder.HasOne(c => c.Department)
            .WithMany(d => d.Courses)
            .HasForeignKey(c => c.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Prerequisite)
            .WithMany()
            .HasForeignKey(c => c.PrerequisiteId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}