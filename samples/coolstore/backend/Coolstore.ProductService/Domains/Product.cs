using System;
using SimpleKit.Domain.Entities;

namespace Coolstore.ProductService.Domains
{
    public class Product : AggregateRootWithId<Guid>
    {
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}