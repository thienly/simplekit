using MediatR;

namespace ProductMgt.ApplicationService
{
    public abstract class PaginatedQuery<TResponse> : IRequest<TResponse> where TResponse: PaginatedResult
    {
        public int PageIndex { get; set; }
        public int RecordsPerPage { get; set; }
    }
}