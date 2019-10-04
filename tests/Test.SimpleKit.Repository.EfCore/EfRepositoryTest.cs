using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Repository;
using Test.SimpleKit.Repository.EfCore.SeedDb;
using Xunit;
using Xunit.Abstractions;

namespace Test.SimpleKit.Repository.EfCore
{
    public class EfRepositoryTest : TestWithSqlite
    {
        private ITestOutputHelper _testOutputHelper;
        private IUnitOfWork _unitOfWork;
        private IDbContextTransaction _transaction;
        public EfRepositoryTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _unitOfWork = new EfUnitOfWork(SuiteDbContext,SuiteDbContext.Database.BeginTransaction(),new NullLogger<EfUnitOfWork>());
        }

        [Fact]
        public async Task Test_if_repository_can_add_with_transaction()
        {
            var personRepository = _unitOfWork.Repository<Person>();
            var p = Fixture.Create<Person>();
            p.RegisterAddress(Fixture.Create<Address>());
            p.AddBankAccount(Fixture.Create<BankAccount>());
            var addedPerson = await personRepository.AddAsync(p);
            var savedChanges = await _unitOfWork.SaveChangesAsync(default(CancellationToken));
            var tracker = SuiteDbContext.ChangeTracker.Entries().ToList();
            Assert.True(tracker.Count > 0);
            var person = SuiteDbContext.Person.Find(addedPerson.Id);
            _testOutputHelper.WriteLine(JsonConvert.SerializeObject(person));
            Assert.NotNull(person);
            Assert.Equal(1,savedChanges);
        }

        [Fact]
        public async Task Test_if_repository_can_rollback_after_added()
        {
            //Arrange
            var personRepository = _unitOfWork.Repository<Person>();
            var p = Fixture.Create<Person>();
            //Act
            var addedPerson = await personRepository.AddAsync(p);
            _transaction.Rollback();
            //Assert
            var person = SuiteDbContext.Person.Find(p.Id);
            Assert.Null(person);
        }
    }
}