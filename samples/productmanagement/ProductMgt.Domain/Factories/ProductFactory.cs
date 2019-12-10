namespace ProductMgt.Domain.Factories
{
    public class ProductFactory : IProductFactory
    {
        public Product CreateProduct(string name, decimal price)
        {
            return new Product(name, price);
        }
    }
}