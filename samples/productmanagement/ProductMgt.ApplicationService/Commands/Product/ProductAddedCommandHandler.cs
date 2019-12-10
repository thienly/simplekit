using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductMgt.Domain.Factories;
using SimpleKit.Domain.Repositories;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductAddedCommandHandler : IRequestHandler<ProductAddedCommand, ProductAddedCommandResult>
    {
        private readonly IRepository<Domain.Product> _repository;
        private readonly IProductFactory _productFactory;

        public ProductAddedCommandHandler(IRepository<Domain.Product> repository, IProductFactory productFactory)
        {
            _repository = repository;
            _productFactory = productFactory;
        }

        public Task<ProductAddedCommandResult> Handle(ProductAddedCommand request, CancellationToken cancellationToken)
        {
            var product = _productFactory.CreateProduct(request.Name, request.Price);
            _repository.AddAsync(product);
            return Task.FromResult(new ProductAddedCommandResult());
        }
    }
}