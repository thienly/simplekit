using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Db;

namespace SimpleKit.Infrastructure.Repository.EfCore.Repository
{
    public delegate object RepositoryFactory(Type type);

    public sealed class EfUnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly IDbContextTransaction _transaction;
        private readonly ILogger<EfUnitOfWork> _logger;
        private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private RepositoryFactory _repositoryFactory;
        private ILoggerFactory _loggerFactory;
        
        public EfUnitOfWork(AppDbContext dbContext, IDbContextTransaction transaction, ILoggerFactory loggerFactory = null,
            RepositoryFactory repositoryFactory = null)
        {
            _dbContext = dbContext;
            _transaction = transaction;
            _repositoryFactory = repositoryFactory;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory ==
                      null
                ? NullLogger<EfUnitOfWork>.Instance
                : _loggerFactory.CreateLogger<EfUnitOfWork>();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = _dbContext.SaveChangesAsync(cancellationToken);
                _transaction.Commit();
                return result;
            }
            catch (DbUpdateConcurrencyException e)
            {
                _transaction.Rollback();
                _logger.LogError(
                    $"There is concurrency error when trying to commit transaction {_transaction.TransactionId}", e);
                throw;
            }
            catch (DbUpdateException e)
            {
                _transaction.Rollback();
                _logger.LogError($"There is error when trying to commit transaction {_transaction.TransactionId}", e);
                throw;
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                _logger.LogError($"There is error when trying to commit transaction {_transaction.TransactionId}", ex);
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
            catch (DbUpdateConcurrencyException e)
            {
                _transaction.Rollback();
                _logger.LogError(
                    $"There is concurrency error when trying to commit transaction {_transaction.TransactionId}", e);
                throw;
            }
            catch (DbUpdateException e)
            {
                _transaction.Rollback();
                _logger.LogError($"There is error when trying to commit transaction {_transaction.TransactionId}", e);
                throw;
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                _logger.LogError($"There is error when trying to commit transaction {_transaction.TransactionId}", ex);
                throw;
            }
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IAggregateRoot
        {
            if (_repositories.ContainsKey(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as IRepository<TEntity>;
            }
            var repository = _repositoryFactory(typeof(IRepository<TEntity>)) as IRepository<TEntity>;
            _repositories.Add(typeof(TEntity), repository);
            return repository;
        }
    }
}