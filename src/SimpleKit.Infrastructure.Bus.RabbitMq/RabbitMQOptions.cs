namespace SimpleKit.Infrastructure.Bus.RabbitMq
{
    public class RabbitMQOptions
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}