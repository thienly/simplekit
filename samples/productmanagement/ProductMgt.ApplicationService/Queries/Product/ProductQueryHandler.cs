using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SimpleKit.Domain.Repositories;
using SimpleKit.Infrastructure.Repository.EfCore.Extensions;

namespace ProductMgt.ApplicationService.Queries.Product
{
    public class ProductQueryHandler : IRequestHandler<ProductQuery,ProductQueryResult>
    {
        private IQueryRepository<Domain.Product> _queryRepository;

        public ProductQueryHandler(IQueryRepository<Domain.Product> queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<ProductQueryResult> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            var paginatedItem = await _queryRepository.Paging(request.PageIndex,request.RecordsPerPage,p=> true);
            return new ProductQueryResult()
            {
                Products = paginatedItem.Items,
                PageIndex = paginatedItem.PageIndex,
                RecordsPerPage = paginatedItem.RecordsPerPage,
                TotalItems = paginatedItem.TotalItems
            };
        }
    }
}