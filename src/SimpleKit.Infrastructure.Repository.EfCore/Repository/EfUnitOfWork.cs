using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Events;
using SimpleKit.Domain.Repositories;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private IDbContextTransaction _transaction; 
        private ILogger<EfUnitOfWork> _logger;

        public EfUnitOfWork(DbContext dbContext, IDbContextTransaction dbTransaction, ILogger<EfUnitOfWork> logger)
        {
            _dbContext = dbContext;
            _transaction = dbTransaction;
            _logger = logger;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Starting transaction {_transaction.TransactionId}");
                var result = _dbContext.SaveChangesAsync(cancellationToken);
                _transaction.Commit();
                _logger.LogInformation("Ending transaction");
                return result;
            }
            catch (SimpleKitTransactionException e)
            {
                _logger.LogError($"There is error when trying to commit transaction {_transaction.TransactionId}", e);
                throw;
            }
        }

        public int SaveChanges()
        {
            try
            {
                _logger.LogInformation($"Starting transaction {_transaction.TransactionId}");
                var result = _dbContext.SaveChanges();
                _transaction.Commit();
                _logger.LogInformation("Ending transaction");
                return result;
            }
            catch (SimpleKitTransactionException e)
            {
                _logger.LogError($"There is error when trying to commit transaction {_transaction.TransactionId}", e);
                throw;
            }
        }
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;
            }

            var repository = new EfRepository<TEntity>(_dbContext);
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }
    }
}