using System;
using ProductMgt.Domain.Events;
using SimpleKit.Domain.Entities;

namespace ProductMgt.Domain
{
    public class Product : AggregateRootWithId<long>
    {
        private string _name;
        private decimal _price;
        private DateTime _expiredDate;
        internal Product(string name, decimal price)
        {
            _name = name;
            _price = price;
            AddEvent(new ProductAddedEvent()
            {
                ProductId = Id
            });
        }
        public string Name => _name;
        public decimal Price => _price;
        public DateTime? ExpiredDate => _expiredDate;

        public void SetExpiredDate(DateTime dateTime)
        {
            if (dateTime < DateTime.Now)
                throw new Exception("Expired Date must be greater than current date");
            _expiredDate = dateTime;
        }

        public void UpdatePrice(decimal price)
        {
            if (price < 0)
                throw new Exception("Product price is less than 0");
            _price = price;
        }
        public void ChangeName(string newName)
        {
            _name = newName;
            AddEvent(new ProductChangedNameEvent()
            {
                OldName = this.Name,
                NewName = newName
            });
        }
    }
}