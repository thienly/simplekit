using MediatR;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductUpdatedCommand: IRequest<ProductUpdatedCommandResult>
    {
        public long Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
    }
}