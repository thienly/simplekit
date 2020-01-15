using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleKit.Domain.Repositories;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductAddMediaCommandHandler : IRequestHandler<ProductAddMediaCommand, ProductAddMediaCommandResult>
    {
        private IRepository<Domain.Product> _repository;

        public ProductAddMediaCommandHandler(IRepository<Domain.Product> repository)
        {
            _repository = repository;
        }

        public Task<ProductAddMediaCommandResult> Handle(ProductAddMediaCommand request, CancellationToken cancellationToken)
        {
            
            throw new System.NotImplementedException();
        }
    }
}