namespace ProductMgtServices.Dtos
{
    public class ProductResponseItem
    {
        public long Id { get; set; }
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
        public long Id { get; set; }
        public decimal Price { get; set; }
    }

    public class ProductDeletedDto
    {
        public long Id { get; set; }
    }
}