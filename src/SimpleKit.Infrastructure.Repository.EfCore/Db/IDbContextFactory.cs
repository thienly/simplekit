using System;

namespace SimpleKit.Infrastructure.Repository.EfCore.Db
{
    public interface IDbContextFactory
    {
        T Create<T>() where T: AppDbContext;
        AppDbContext Create(Type type);
    }
}