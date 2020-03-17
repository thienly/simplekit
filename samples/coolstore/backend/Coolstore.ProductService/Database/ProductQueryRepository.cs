using System;
using System.Linq;
using Coolstore.ProductService.Domains;
using SimpleKit.Domain.Repositories;

namespace Coolstore.ProductService.Database
{
    public class ProductQueryRepository : IQueryRepository<Product>
    {
        public IQueryable<Product> Queryable()
        {
            throw new NotImplementedException();
        }
    }
    
    
}