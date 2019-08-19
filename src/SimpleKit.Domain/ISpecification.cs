using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SimpleKit.Domain
{
    public interface ISpecification<T>
    {
        Expression<Func<T,bool>> Criteria { get; set; }
        List<Expression<Func<T, object>>> Includes { get; }
    }
}