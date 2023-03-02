using BE.Data.Constand;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE.Data.Configurations
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(s => s.id);
            builder.Property(s => s.id)
                .UseIdentityColumn();
            builder.ToTable(ConfigEntity.Notifications.TABLE_NAME);
            builder.Property(s => s.title)
                .HasMaxLength(ConfigEntity.Notifications.MAX_LENGTH_TITLE)
                .IsRequired();
            builder.Property(s => s.message)
               .HasMaxLength(ConfigEntity.Notifications.MAX_LENGTH_MESSAGE)
               .IsRequired();
        }
    }
}
