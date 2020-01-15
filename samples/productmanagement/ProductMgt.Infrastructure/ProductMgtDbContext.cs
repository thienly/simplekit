using System;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace ProductMgt.Infrastructure
{
    public class ProductMgtDbContext : AppDbContext
    {
        private UserContext.UserContext _userContext;

        public ProductMgtDbContext()
        {
            
        }
        public ProductMgtDbContext(DbContextOptions options) : base(options)
        {
        }

        public UserContext.UserContext UserContext
        {
            get => _userContext;
            set => _userContext = value;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OutBoxMessageEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductMediaEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
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
                        auditable.CreatedBy = _userContext.UserId;
                        auditable.CreatedDate = DateTime.Now;
                    }

                    if (entityEntry.State == EntityState.Modified)
                    {
                        auditable.UpdatedBy = _userContext.UserId;
                        auditable.UpdatedDate = DateTime.Now;
                    }
                }
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=10.0.19.103;Database=ProductMgt;User Id=sa;Password=Test!234",
                    x => x.MigrationsAssembly("ProductMgt.Infrastructure"));
            }

            base.OnConfiguring(optionsBuilder);
        }
    }
}