using System;

namespace ProductMgtServices.Dtos
{
    public class ProductResponseItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
    public class ProductGetAllResponse : PaginatedItems<ProductResponseItem>
    {
        
    }
    public class ProductAddedDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductUpdatedDto
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductDeletedDto
    {
        public Guid Id { get; set; }
    }
}