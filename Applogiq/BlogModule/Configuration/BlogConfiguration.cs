using Applogiq.BlogModule.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Applogiq.BlogModule.Configuration
{
    public class BlogConfiguration : IEntityTypeConfiguration<Blog>
    {
        public void Configure(EntityTypeBuilder<Blog> entity)
        {
            entity.ToTable("Blog");


            entity.HasKey(e => e.Id);


            entity.Property(e => e.Title).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.PublishDate).IsRequired();
            entity.Property(e => e.Author).IsRequired().HasMaxLength(128);

            entity.Property(e => e.CreatedBy).IsRequired().HasMaxLength(128);
            entity.Property(e => e.ModifiedBy).HasMaxLength(128);

            entity.HasMany(b => b.Comments)
                  .WithOne(c => c.Blog)
                  .HasForeignKey(c => c.BlogId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
}