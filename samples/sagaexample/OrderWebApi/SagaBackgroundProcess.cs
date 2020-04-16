using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderWebApi.Domains;
using SimpleKit.StateMachine;

namespace OrderWebApi
{
    public class TimerBackgroundProcess : IHostedService, IDisposable
    {
        private Timer _timer;
        private IServiceProvider _serviceProvider;
        private ILogger<TimerBackgroundProcess> _logger;

        public TimerBackgroundProcess(IServiceProvider serviceProvider, ILogger<TimerBackgroundProcess> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saga background job is starting");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var databaseAdapter = scope.ServiceProvider.GetService<IDatabaseAdapter<Order>>();
                var uncompletedOrders = databaseAdapter.GetUncompletedOrders();
                    foreach (var order in uncompletedOrders)
                {
                    var maxVersion = order.States.Max(x => x.Version);
                    var sagaStateProxy = order.States.Single(x => x.Version == maxVersion);
                    var sagaManager = scope.ServiceProvider.GetService<ISagaManager<BookingTripState>>();
                    var stateProxy = sagaManager.Process((BookingTripState) sagaStateProxy.SagaState, sagaStateProxy);
                    order.AddState(stateProxy);
                    databaseAdapter.Update(order);
                }
            }
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saga background job is stopped");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}