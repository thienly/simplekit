using MediatR;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductAddMediaCommand : IRequest<ProductAddMediaCommandResult>
    {
        public long ProductId { get; set; }
        public byte[] Data { get; set; }
    }
}