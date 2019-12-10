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
            builder.Property(x => x.Name).HasField("_name");
            builder.Property(x => x.Price).HasField("_price");
            builder.Property(x => x.ExpiredDate).HasField("_expiredDate");
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
            builder.Property(x => x.Type).HasColumnName("varchar(500)").HasConversion(t => t.FullName, s => Type.GetType(s)).IsRequired();
            builder.Property(x => x.Body).HasColumnType("varchar(500)").IsRequired();
            builder.Property(x => x.DispatchedTime).IsRequired();
            builder.Property(x => x.ProcessedTime).IsRequired();
            builder.AddConcurrencyToken();
        }
    }
}