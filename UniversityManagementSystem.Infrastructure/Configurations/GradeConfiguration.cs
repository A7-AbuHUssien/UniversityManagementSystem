using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Infrastructure.Configurations;

public class GradeConfiguration : IEntityTypeConfiguration<Grade>
{
    public void Configure(EntityTypeBuilder<Grade> builder)
    {
        builder.Property(g => g.Symbol).IsRequired().HasMaxLength(5);
        builder.Property(g => g.Description).HasMaxLength(100);
        
        builder.Property(g => g.MinScore).HasPrecision(5, 2);
        builder.Property(g => g.MaxScore).HasPrecision(5, 2);
        builder.Property(g => g.GPAPoint).HasColumnType("decimal(18,2)");
    }
}