using System;
using System.Collections.Generic;
using ProductMgt.Domain.Events;
using SimpleKit.Domain.Entities;

namespace ProductMgt.Domain
{
    public class Product : AggregateRootBase
    {
        private string _name;
        private decimal _price;
        private DateTime _expiredDate;
        private ICollection<ProductMedia> _productMedia = new List<ProductMedia>();
        internal Product(string name, decimal price)
        {
            _name = name;
            _price = price;
        }
        public string Name => _name;
        public decimal Price => _price;
        public DateTime? ExpiredDate => _expiredDate;

        public ICollection<ProductMedia> ProductMedia
        {
            get => _productMedia;
            set => _productMedia = value;
        }

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
        public void AddMedia(ProductMediaType type, string relativePath)
        {
            var media = new ProductMedia(this.Id, type, relativePath);
            ProductMedia.Add(media);
        }
    }
}