using System.Threading.Tasks;
using Grpc.Core;
using MediatR;
using ProductServices;

namespace Coolstore.Services
{
    public class ProductServices : ProductSvc.ProductSvcBase
    {
        private IMediator _mediator;

        public ProductServices(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override Task<ProductData> GetProduct(ProductData request, ServerCallContext context)
        {
            throw new RpcException(new Status(StatusCode.Unknown, "Unknown"),"Hello world");
        }
        
    }
}
