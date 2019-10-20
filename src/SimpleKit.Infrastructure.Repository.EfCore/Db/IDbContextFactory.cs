using System;
using System.Threading.Tasks;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore
{
    public interface IDbContextFactory
    {
        T Create<T>() where T: AppDbContext;
        AppDbContext Create(Type type);
    }
}