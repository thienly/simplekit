using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain;

namespace SimpleKit.Repository.EfCore
{
    public class EfUnitOfWork :IUnitOfWork
    {
        private readonly DbContext _dbContext;

        public EfUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}