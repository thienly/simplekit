using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Extensions;

namespace ProductMgt.ApplicationService.Commands.Product
{
    public class ProductDeletedCommandHandler : IRequestHandler<ProductDeletedCommand, ProductDeletedCommandResult>
    {
        private IRepository<Domain.Product> _repository;
        private IQueryRepository<Domain.Product> _queryRepository;

        public ProductDeletedCommandHandler(IRepository<Domain.Product> repository, IQueryRepository<Domain.Product> queryRepository)
        {
            _repository = repository;
            _queryRepository = queryRepository;
        }

        public async Task<ProductDeletedCommandResult> Handle(ProductDeletedCommand request, CancellationToken cancellationToken)
        {
            var found = await _queryRepository.FirstOrDefault(x=>x.Id == request.Id);
            await _repository.DeleteAsync(found);
            return new ProductDeletedCommandResult();
        }
    }
}