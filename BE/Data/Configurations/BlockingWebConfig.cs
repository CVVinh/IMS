using BE.Data.Constand;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace BE.Data.Configurations
{
    public class BlockingWebConfig : IEntityTypeConfiguration<BlockingWeb>
    {
        public void Configure(EntityTypeBuilder<BlockingWeb> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.BlockingWeb.TABLE_NAME);
            builder.Property(s => s.linkBlockingWeb)
                .IsRequired();
        }
    }
}
