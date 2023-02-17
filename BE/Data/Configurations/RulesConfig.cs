using BE.Data.Constand;
using BE.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BE.Data.Configurations
{
	public class RulesConfig : IEntityTypeConfiguration<Rules>
	{
		public void Configure(EntityTypeBuilder<Rules> builder)
		{
			builder.HasKey(s => s.id);
			builder.Property(s => s.id)
				.UseIdentityColumn();
			builder.ToTable(ConfigEntity.Rules.TABLE_NAME);
			builder.Property(s => s.title)
				.HasMaxLength(ConfigEntity.Rules.MAX_LENGTH_TITLE)
				.IsRequired();
		}
	}
}
