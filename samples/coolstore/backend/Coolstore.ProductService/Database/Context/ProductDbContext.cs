using System;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace Coolstore.ProductService.Database.Context
{
    public class ProductDbContext: AppDbContext
    {
        public ProductDbContext(DbContextOptions options) : base(options)
        {
        }
        public override IDomainEventDispatcher DomainEventDispatcher { get; set; }
        public override void PreProcessSaveChanges()
        {
            foreach (var entityEntry in this.ChangeTracker.Entries())
            {
                if (entityEntry.Entity is IAuditable)
                {
                    var auditable = (IAuditable) entityEntry.Entity;
                    if (entityEntry.State == EntityState.Added)
                    {
                        auditable.CreatedBy = 999;
                        auditable.CreatedDate = DateTime.Now;
                    }

                    if (entityEntry.State == EntityState.Modified)
                    {
                        auditable.UpdatedBy = 999;
                        auditable.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}