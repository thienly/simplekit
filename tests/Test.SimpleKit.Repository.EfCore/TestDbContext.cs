using System.Threading.Tasks;
using Test.SimpleKit.Repository.EfCore.SeedDb;
using Xunit;

namespace Test.SimpleKit.Repository.EfCore
{
    public class TestDbContext : TestWithSqlite
    {
        [Fact]
        public async Task Test_if_can_create_db_context_successfully()
        {
            Assert.True(await SuiteDbContext.Database.CanConnectAsync());
        }

        [Fact]
        public async Task Test_if_add_return_new_id()
        {
            var p = new Person(1,"TLP");
            p.RegisterAddress(new Address()
            {
                Ward = "P1",
                Street = "TL10",
                AddressNumber = "123"
            });
            SuiteDbContext.Person.Add(p);
            SuiteDbContext.SaveChanges();
            var person = SuiteDbContext.Person.Find(1);
            Assert.NotNull(person);
        }
    }
}