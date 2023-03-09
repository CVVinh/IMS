using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE.Data.Configurations
{
    public class StaffReviewConfig : IEntityTypeConfiguration<StaffReview>
    {
        public void Configure(EntityTypeBuilder<StaffReview> builder)
        {
            builder.HasKey(x => x.id);
            builder.Property(x => x.id)
                .UseIdentityColumn();
            builder.Property(x => x.achievements).HasColumnType("text");
            builder.Property(x => x.knowledge).HasColumnType("text");
            builder.Property(x => x.skill).HasColumnType("text");
            builder.Property(x => x.forwardMindset).HasColumnType("text");
            builder.Property(x => x.positiveMindset).HasColumnType("text");
            builder.Property(x => x.steadfastMindset).HasColumnType("text");
            builder.Property(x => x.perfectionistMindset).HasColumnType("text");
            builder.Property(x => x.fromTalkToResult).HasColumnType("text");
            builder.Property(x => x.connectToAction).HasColumnType("text");
            builder.Property(x => x.hobbies).HasColumnType("text");
            builder.Property(x => x.personality).HasColumnType("text");
            builder.Property(x => x.commitmentsForCompany).HasColumnType("text");
            builder.Property(x => x.colleagueOpinion).HasColumnType("text");
            builder.Property(x => x.HROpinion).HasColumnType("text");
        }
    }
}
