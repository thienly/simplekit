using System.Collections.Generic;
using SimpleKit.Domain.Entities;

namespace SimpleKit.Domain.Utilities
{
    public class PaginatedItem<TEntity> where TEntity: class, IAggregateRoot
    {
        public PaginatedItem(int totalItems, int index, int recordsPerPage, List<TEntity> items)
        {
            TotalItems = totalItems;
            PageIndex = index;
            Items = items;
            RecordsPerPage = recordsPerPage;
        }
        public int RecordsPerPage { get; }
        public int TotalItems { get; }
        public int PageIndex { get; }
        public List<TEntity> Items { get; }
    }
}