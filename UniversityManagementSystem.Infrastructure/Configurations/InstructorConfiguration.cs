using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Infrastructure.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.Email)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(i => i.Email)
            .IsUnique();

        builder.Property(i => i.Specialization)
            .HasMaxLength(100);

        builder.HasOne(i => i.Department)
            .WithMany(d => d.Instructors)
            .HasForeignKey(i => i.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict); 
    }
}