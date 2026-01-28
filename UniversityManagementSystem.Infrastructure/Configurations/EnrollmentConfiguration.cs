using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Infrastructure.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.StudentId, e.CourseId, e.SemesterId })
            .IsUnique();

        builder.Property(e => e.EnrollmentDate)
            .HasDefaultValueSql("GETDATE()");

        builder.Property(e => e.NumericScore)
            .HasColumnType("decimal(5,2)");

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.StudentId);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId);

        builder.HasOne(e => e.Grade)
            .WithMany(g => g.Enrollments)
            .HasForeignKey(e => e.GradeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(e => e.Status)
            .HasDefaultValue(EnrollmentStatus.Active)
            .HasSentinel((EnrollmentStatus)(-1));
    }
}