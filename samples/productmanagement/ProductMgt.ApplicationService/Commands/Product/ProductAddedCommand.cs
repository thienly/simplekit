using MediatR;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductAddedCommand: IRequest<ProductAddedCommandResult>
    {
        public ProductAddedCommand(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}