using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Castle.MicroKernel.Lifestyle;
using Castle.MicroKernel.Registration;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Repository;
using Test.SimpleKit.Domain.SeedDb;
using Test.SimpleKit.Repository.EfCore.DbContext;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Repository.EfCore
{
    public class EfRepositoryTest : TestWithSqlite
    {
        private ITestOutputHelper _testOutputHelper;
        private IUnitOfWork _unitOfWork;
        private IDbContextTransaction _transaction;
        public EfRepositoryTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test_if_repository_can_add_with_transaction()
        {
            _container.Register(Component.For<IDbContextTransaction>()
                .Instance(SuiteDbContext.Database.BeginTransaction()).LifestyleScoped());
            _container.Register(Component.For<IUnitOfWork>().ImplementedBy<EfUnitOfWork>().LifestyleScoped());
            _container.Register(Component.For(typeof(ILogger<>)).ImplementedBy(typeof(NullLogger<>)).LifestyleScoped());
            _container.Register(Component.For<Microsoft.EntityFrameworkCore.DbContext>().Instance(SuiteDbContext).LifestyleScoped());
            _container.Register(Classes.FromAssembly(Assembly.GetAssembly(typeof(Domain.DataModels.Address)))
                .BasedOn(typeof(IDomainEventHandler<>)).WithServiceAllInterfaces().LifestyleScoped());
            using (var scope = _container.BeginScope())
            {
                var unitOfWork = _container.Resolve<IUnitOfWork>();
                new DomainEvents(t => _container.ResolveAll(t).Cast<object>());
                var personRepository = unitOfWork.Repository<Person>();

                var p = Fixture.Create<Person>();
                p.RegisterAddress(Fixture.Create<Address>());
                p.AddBankAccount(Fixture.Create<BankAccount>());
                var addedPerson = await personRepository.AddAsync(p);
                var savedChanges = await unitOfWork.SaveChangesAsync(default(CancellationToken));
                var person = SuiteDbContext.Person.Find(addedPerson.Id);
                Assert.NotNull(person);
            }
        }
    }
}