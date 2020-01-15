using System;
using System.Linq;
using System.Reflection;
using AutoFixture;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleKit.Domain;
using SimpleKit.Domain.Events;
using SimpleKit.Infrastructure.Repository.EfCore.Db;
using Test.SimpleKit.Base;
using Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base.SeedDb;
using Xunit.Abstractions;

namespace Test.SimpleKit.Infrastructure.Repository.EfCore.MySQL.Base
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
    public class SuiteMySQLDbContext : AppDbContext
    {
        public DbSet<Person> Person { get; set; }
        public DbSet<BankAccount> BankAccount { get; set; }
        public SuiteMySQLDbContext(DbContextOptions options): base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new BankAccountEntityTypeConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public override IDomainEventDispatcher DomainEventDispatcher { get; set; }

        public override void PreProcessSaveChanges()
        {
        }
    }
    public class TestWithMySQL: SimpleKitTestBase
    {
        private string _connectionString = "server=localhost;port=3306;user=root;password=new;database=TestDB";
        private ITestOutputHelper _testOutputHelper;
        protected TestWithMySQL(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        public IDisposable SeedData(SuiteMySQLDbContext data)
        {
            var persons = Fixture.Build<Person>().FromFactory(() =>
            {
                var p = new Person("Name");
                p.RegisterAddress(Fixture.Create<Address>());
                return p;
            }).CreateMany<Person>(100).ToList();
            data.Set<Person>().AddRange(persons);
            data.SaveChanges();
            return new CleanupResource(data);
        }

        public IDisposable InitializeDatabase(SuiteMySQLDbContext suiteMySqlDbContext)
        {
            suiteMySqlDbContext.Database.EnsureCreated();
            return new CleanupResource(suiteMySqlDbContext);
        }
        internal class CleanupResource : IDisposable
        {

            private SuiteMySQLDbContext _mySqlDbContext;

            public CleanupResource(SuiteMySQLDbContext mySqlDbContext)
            {
                _mySqlDbContext = mySqlDbContext;
            }

            public void Dispose()
            {
                if (_mySqlDbContext.Database.CanConnect())
                {
                    _mySqlDbContext.Database.CloseConnection();
                    _mySqlDbContext.Database.EnsureDeleted();
                }
            }
        }
    }
    
}