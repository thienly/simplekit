using System;
using ProductMgtServices.Domains.Events;
using SimpleKit.Domain.Entities;

namespace ProductMgtServices.Domains
{
    public class Product : AggregateRootWithId<long>, ICloneable
    {
        internal Product(string name, decimal price)
        {
            Name = name;
            Price = price;
            AddEvent(new ProductAddedEvent()
            {
                Product = (Product)Clone()
            });
        }
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        
        public DateTime? ExpiredDate { get; private set; }
        public long CategoryId { get; private set; }

        public void AssignCategory(long categoryId)
        {
            CategoryId = categoryId;
            AddEvent(new ProductAssignedCategoryEvent()
            {
                CategoryId = categoryId
            });
        }

        public void ChangeName(string newName)
        {
            Name = newName;
            AddEvent(new ProductChangedNameEvent()
            {
                OldName = this.Name,
                NewName = newName
            });
        }
        public object Clone()
        {
            return new Product(this.Name,this.Price);
        }
    }
}