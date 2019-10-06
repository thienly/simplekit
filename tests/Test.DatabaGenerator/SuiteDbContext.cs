using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleKit.Domain;
using Test.SimpleKit.Domain.SeedDb;

namespace Test.DatabaGenerator
{
    public static class EntityTypeConfigurationHelper
    {
        public static void IgnoreAuditInformation<T>(this EntityTypeBuilder<T> entity) where T: class
        {
            var audit = entity.GetType().GenericTypeArguments[0] as IAuditable;
            if (audit != null)
            {
                var propertyInfos = typeof(IAuditable).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    entity.Ignore(propertyInfo.Name);
                }
            }
        }
    }
    public class PersonEntityTypeConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.IgnoreAuditInformation();
            builder.OwnsOne(x => x.PermenantAddress,a=>
            {
                a.ToTable("Address");
                a.Property("PersonId").ValueGeneratedNever();
                a.Property(x => x.Ward).HasMaxLength(100).IsRequired();
                a.Property(x => x.Street).HasMaxLength(100).IsRequired();
                a.Property(x => x.AddressNumber).HasMaxLength(100).IsRequired();
                
            });
            builder.HasMany(x => x.BankAccounts).WithOne(e=> e.Person);
        }
    }

    public class BankAccountEntityTypeConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.ToTable("BankAccount");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BankName).HasMaxLength(100).IsRequired();
        }
    }
    public class SuiteDbContext : DbContext
    {
        public SuiteDbContext()
        {
            
        }
        public DbSet<Person> Person { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public SuiteDbContext(DbContextOptions<SuiteDbContext> options) : base(options)
        {
            
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            optionsBuilder.UseSqlServer("Server=localhost;Database=EfCore;User Id=sa;Password=Test!234;");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BankAccountEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}