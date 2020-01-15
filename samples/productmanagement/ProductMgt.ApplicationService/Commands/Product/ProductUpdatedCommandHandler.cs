using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
            var product = await _queryRepository.FirstOrDefault(x => x.Id == request.Id);
            if (product == null)
                throw new AppServiceException($"The product with Id: {request.Id} can not be found");
            product.UpdatePrice(request.Price);
            var updateAsync = await _repository.UpdateAsync(product);
            return new ProductUpdatedCommandResult();
        }
    }
}