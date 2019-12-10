using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ProductMgt.Domain.Factories;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Extensions;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductUpdatedCommandHandler : IRequestHandler<ProductUpdatedCommand, ProductUpdatedCommandResult>
    {
        private readonly IRepository<Domain.Product> _repository;
        private readonly IQueryRepository<Domain.Product> _queryRepository;

        public ProductUpdatedCommandHandler(IRepository<Domain.Product> repository, IQueryRepository<Domain.Product> queryRepository)
        {
            _repository = repository;
            _queryRepository = queryRepository;
        }

        public async Task<ProductUpdatedCommandResult> Handle(ProductUpdatedCommand request, CancellationToken cancellationToken)
        {
            var products = await _queryRepository.Find(x => x.Id == request.Id, null);
            var product = products.FirstOrDefault();
            if (product == null)
                throw new Exception("Can not find product");
            product.UpdatePrice(request.Price);
            var updateAsync = await _repository.UpdateAsync(product);
            return new ProductUpdatedCommandResult();
        }
    }
}