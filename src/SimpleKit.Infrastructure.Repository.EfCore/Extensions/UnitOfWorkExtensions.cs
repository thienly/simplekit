using System;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SimpleKit.Domain.Repositories;


namespace SimpleKit.Infrastructure.Repository.EfCore.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static IUnitOfWork SetTimeOut(this IUnitOfWork unitOfWork, DbContext dbContext, TimeSpan timeSpan)
        {
            dbContext.Database.SetCommandTimeout(timeSpan);
            return unitOfWork;
        }

        public static int? GetTimeOut(this IUnitOfWork unitOfWork, DbContext dbContext)
        {
            return dbContext.Database.GetCommandTimeout();
        }

        public static int ExecuteSqlCommand(this IUnitOfWork unitOfWork, DbContext dbContext, string sqlCommand,
            params DbParameter[] parameters)
        {
            return dbContext.Database.ExecuteSqlRaw(sqlCommand, parameters);
        }

        public static Task<int> ExecuteSqlCommandAsync(this IUnitOfWork unitOfWork, DbContext dbContext,
            string sqlCommand, params DbParameter[] parameters)
        {
            return dbContext.Database.ExecuteSqlRawAsync(sqlCommand, parameters);
        }
    }
}