using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Order.HotelService.Consumers
{
    public class SagaBackgroundService : BackgroundService
    {
        private SagaHotelConsumer _hotelConsumer;
        private ILogger<SagaBackgroundService> _logger;

        public SagaBackgroundService(SagaHotelConsumer hotelConsumer, ILogger<SagaBackgroundService> logger)
        {
            _hotelConsumer = hotelConsumer;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Saga background service is started");
            _hotelConsumer.Consume("saga-book-hotel-queue");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Saga background service is stopped");
            return base.StopAsync(cancellationToken);
        }
    }
}