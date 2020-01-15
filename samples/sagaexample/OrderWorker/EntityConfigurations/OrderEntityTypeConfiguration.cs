using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SimpleKit.StateMachine;
using SimpleKit.StateMachine.Definitions;

namespace OrderWorker.EntityConfigurations
{
    public class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Domains.Order>
    {
        public void Configure(EntityTypeBuilder<Domains.Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasField("_name");
            builder.Property(x => x.CreatedDate);
            builder.Property(x => x.Status);
        }
    }

    public class SagaStateEntityConfiguration : IEntityTypeConfiguration<SagaStateProxy>
    {
        public void Configure(EntityTypeBuilder<SagaStateProxy> builder)
        {
            builder.ToTable("SagaState");
            builder.HasKey(x => new {x.SagaId, x.Version});
            builder.Property(x => x.SagaId).IsRequired();
            builder.Property(x => x.SagaState).HasColumnName("Data").HasConversion(state =>
                JsonConvert.SerializeObject(state, new JsonSerializerSettings()
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                    TypeNameHandling = TypeNameHandling.All
                }), st => JsonConvert.DeserializeObject<ISagaState>(st,new JsonSerializerSettings()
            {
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full,
                TypeNameHandling = TypeNameHandling.All
            })).IsRequired();
            builder.Property(x => x.CurrentState).HasColumnName("CurrentState").IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.NextState);
            builder.Property(x => x.IsCompleted).IsRequired();
            builder.Property(x => x.CreatedDate).IsRequired();
            builder.Property(x => x.SagaDefinitionType).HasConversion(t => t.AssemblyQualifiedName, 
                str => Type.GetType(str));
            builder.Property(x => x.Direction).IsRequired();
        }
    }
    
    public class OrderDbContext : DbContext
    {
        public OrderDbContext()
        {
            
        }
        public OrderDbContext(DbContextOptions options) : base(options) 
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=10.0.19.103;Database=OrderMgt;User Id=sa;Password=Test!234");
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SagaStateEntityConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}