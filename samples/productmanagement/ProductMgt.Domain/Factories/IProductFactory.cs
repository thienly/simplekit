namespace ProductMgt.Domain.Factories
{
    public interface IProductFactory
    {
        Product CreateProduct(string name, decimal price);
    }
}