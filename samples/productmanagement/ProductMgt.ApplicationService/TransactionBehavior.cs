using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SimpleKit.Domain.Repositories;

namespace ProductMgt.ApplicationService
{
    public class TransactionBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
                
        }

        public ILogger<TransactionBehavior<TRequest, TResponse>> Logger { get; set; } = NullLogger<TransactionBehavior<TRequest, TResponse>>.Instance;
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                using (_unitOfWork)
                {
                    Logger.LogInformation("[TransactionBehavior] Starting the transaction");
                    var response = await next();
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return response;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"[TransactionBehavior] Transaction is aborted with error : {e.Message}");
                throw;
            }
        }
    }
}