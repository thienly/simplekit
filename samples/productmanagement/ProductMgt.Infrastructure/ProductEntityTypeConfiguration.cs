using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductMgt.Domain;

namespace ProductMgt.Infrastructure
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseHiLo("ProductSequenceHilo");
            builder.Property(x => x.Name).HasField("_name");
            builder.Property(x => x.Price).HasField("_price");
            builder.Property(x => x.ExpiredDate).HasField("_expiredDate");
            builder.HasMany(x => x.ProductMedia);
            builder.AddAuditMapping();
            builder.AddConcurrencyToken();
        }
    }

    public class OutBoxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessage");
            builder.HasKey(x => x.Id);
            builder.AddAuditMapping();
            builder.Property(x => x.Type).HasColumnName("DataType").HasConversion(t => t.FullName, s => Type.GetType(s)).IsRequired();
            builder.Property(x => x.Body).HasColumnType("varchar(500)").IsRequired();
            builder.Property(x => x.DispatchedTime).IsRequired();
            builder.Property(x => x.ProcessedTime).IsRequired();
            builder.Property(x => x.NumberOfRetries).HasDefaultValue(0);
            builder.AddConcurrencyToken();
        }
    }

    public class ProductMediaEntityTypeConfiguration : IEntityTypeConfiguration<ProductMedia>
    {
        public void Configure(EntityTypeBuilder<ProductMedia> builder)
        {
            builder.ToTable("ProductMedia");
            builder.AddAuditMapping();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.MediaType).HasConversion<string>();
            builder.Property(x => x.RelativePath).IsRequired();
        }
    }
}