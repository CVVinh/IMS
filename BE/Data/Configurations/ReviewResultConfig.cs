using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE.Data.Configurations
{
    public class ReviewResultConfig : IEntityTypeConfiguration<ReviewResult>
    {
        public void Configure(EntityTypeBuilder<ReviewResult> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.id)
                .UseIdentityColumn();
        }
    }
}
