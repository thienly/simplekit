using System.Collections.Generic;

namespace ProductMgtServices.Dtos
{
    public class PaginatedItems<TEntity>
    {
        public int PageIndex { get; set; }
        public int TotalItems { get; set; }
        public int RecordsPerPage { set; get; }
        public ICollection<TEntity> Items { get; set; } = new List<TEntity>();
    }

    public abstract class PaginatedQueryParameter
    {
        public int PageIndex { get; set; }
        public int RecordsPerPage { set; get; }
    }
}