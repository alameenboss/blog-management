using Applogiq.BlogModule.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Applogiq.BlogModule.Configuration
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> entity)
        {
            entity.ToTable("Comment"); 

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(128);
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);

            entity.HasOne(c => c.Blog)
                  .WithMany(b => b.Comments)
                  .HasForeignKey(c => c.BlogId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}