using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SimpleKit.Domain;

namespace SimpleKit.Repository.EfCore
{
    public class EfSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; }
    }
}