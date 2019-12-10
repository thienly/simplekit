namespace ProductMgtServices.Domains.Factories
{
    public interface IProductFactory
    {
        Product CreateProduct(string name, decimal price);
    }
}