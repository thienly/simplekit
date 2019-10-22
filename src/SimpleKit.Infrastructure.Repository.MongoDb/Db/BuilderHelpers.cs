using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MongoDB.Driver;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Infrastructure.Repository.MongoDb.Db
{
    public static class BuilderHelpers
    {
        public static FilterDefinition<TDocument> RequiredFilter<TDocument, TKey>(TDocument document)
            where TDocument : AggregateRootWithId<TKey>
        {
            var builder = Builders<TDocument>.Filter;
            return builder.Eq("Id", document.Id) & builder.Eq("Version", document.Version);
        }
        public static FilterDefinition<TDocument> RequiredFilter<TDocument, TKey>(IEnumerable<TDocument> documents)
            where TDocument : AggregateRootWithId<TKey>
        {
            var lst = new List<FilterDefinition<TDocument>>();
            var builder = Builders<TDocument>.Filter;
            foreach (var document in documents)
            {
                lst.Add(builder.Eq("Id", document.Id) & builder.Eq("Version", document.Version));
            }

            return builder.Or(lst);
        }
        public static Expression<Func<TDocument, object>> ConvertExpression<TDocument, TValue>(Expression<Func<TDocument, TValue>> expression)
        {
            var param = expression.Parameters[0];
            Expression body = expression.Body;
            var convert = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<TDocument, object>>(convert, param);
        }
    }
}