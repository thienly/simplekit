using System.Collections.Generic;

namespace ProductMgt.ApplicationService.Queries.Product
{
    public class ProductQueryResult: PaginatedResult
    {
        public ICollection<Domain.Product> Products { get; set; } = new List<Domain.Product>();
    }
}