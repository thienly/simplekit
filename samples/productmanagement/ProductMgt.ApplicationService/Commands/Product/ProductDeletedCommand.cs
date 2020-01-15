using MediatR;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductDeletedCommand: IRequest<ProductDeletedCommandResult>
    {
        public long Id { get; set; }
    }
}