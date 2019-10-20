using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using SimpleKit.Domain.Entities;
using SimpleKit.Domain.Repositories;
using SimpleKit.Domain.Utilities;

namespace SimpleKit.Infrastructure.Repository.EfCore.Extensions
{
    /// <summary>
    /// Extensions only support the general case. For specific case such as findbyid.Repeatly, Repository is the place doing the mapping between
    /// domain model and data model(data schema)
    /// </summary>
    public static class RepositoryExtensions
    {
        public static Task<List<TEntity>> Find<TEntity>(this IQueryRepository<TEntity> repository,
            Expression<Func<TEntity, bool>> filter, List<Expression<Func<TEntity, object>>> includes)
        where TEntity: class, IAggregateRoot
        {
            var queryable = repository.Queryable().AsNoTracking();
            var includableQueryable = includes.Aggregate(queryable,((current, include) => current.Include(include)));
            var aggregateRoots = includableQueryable.Where(filter).ToList();
            return Task.FromResult(aggregateRoots);
        }
        public static Task<PaginatedItem<TEntity>> Paging<TEntity>(this IQueryRepository<TEntity> repository,
            int pageIndex, int numberOfItemsPerPage, Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>,IIncludableQueryable<TEntity,object>> includes = null, Func<IQueryable<TEntity>,IOrderedQueryable<TEntity>> orderby = null)
            where TEntity: class, IAggregateRoot
        {
            var aggregateRoots = repository.Queryable();
            if (includes != null)
            {
                aggregateRoots = includes(aggregateRoots);
            }

            if (orderby != null)
            {
                aggregateRoots = orderby(aggregateRoots);
            }    
            var totalItems = aggregateRoots.Count();
            var entities = aggregateRoots.Skip((pageIndex + 1) * numberOfItemsPerPage).Take(numberOfItemsPerPage)
                .Where(filter).ToList();
            return Task.FromResult(new PaginatedItem<TEntity>(totalItems, pageIndex,
                numberOfItemsPerPage, entities));
        }
    }
}