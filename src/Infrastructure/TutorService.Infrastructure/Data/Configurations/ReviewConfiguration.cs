using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutorService.Domain.Entities;

namespace TutorService.Infrastructure.Data.Configurations;

public class ReviewConfiguration : BaseEntityConfiguration<Review>
{
    public override void Configure(EntityTypeBuilder<Review> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Reviews");
        
        builder.Property(r => r.Text)
            .IsRequired()
            .HasMaxLength(2000);
        
        builder.Property(r => r.Rating)
            .IsRequired();
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(r => r.TutorProfile)
            .WithMany(t => t.Reviews)
            .HasForeignKey(r => r.TutorProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(r => new { r.UserId, r.TutorProfileId })
            .IsUnique();
        
        builder.HasIndex(r => r.TutorProfileId);
    }
}