using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleKit.Domain;
using SimpleKit.Domain.Entities;

namespace ProductMgt.Infrastructure
{
    public static class EntityTypeConfigurationExtensions
    {
        public static void AddAuditMapping<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IAuditable
        {
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired();
            builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate").IsRequired();
            builder.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy");
            builder.Property(x => x.UpdatedDate).HasColumnName("UpdateDate");
        }
        public static void AddConcurrencyToken<TEntity>(this EntityTypeBuilder<TEntity> builder) where TEntity : class, IAggregateRoot
        {
            builder.Property(x => x.Version).IsRowVersion();
        } 
    }
}