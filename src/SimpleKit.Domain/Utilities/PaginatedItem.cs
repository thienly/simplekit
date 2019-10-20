using System.Collections.Generic;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Domain.Utilities
{
    public class PaginatedItem<TEntity> where TEntity: class, IAggregateRoot
    {
        public PaginatedItem(int totalItems, int index, int numberOfItemsPerPage, List<TEntity> items)
        {
            TotalItems = totalItems;
            PageIndex = index;
            Items = items;
            NumberOfItemsPerPage = numberOfItemsPerPage;
        }
        public int NumberOfItemsPerPage { get; }
        public int TotalItems { get; }
        public int PageIndex { get; }
        public int PageNumber => TotalItems / NumberOfItemsPerPage; 
        public List<TEntity> Items { get; }
    }
}