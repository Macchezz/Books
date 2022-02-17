using System.Text;
using RabbitMQ.Client;

namespace test.Modules.Books;

public interface IBookMessageRepository
{
    void Send(string message, string queue, string exchange);
}
public class BookMessageRepository : IBookMessageRepository
{
    public void Send(string message, string queue, string exchange)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
            using(var connectionRabbit = factory.CreateConnection())
            using(var channel = connectionRabbit.CreateModel())
            {
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.QueueDeclare(queue: queue,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                
                channel.ExchangeDeclare(exchange, ExchangeType.Topic);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: exchange,
                                    routingKey: queue,
                                    basicProperties: properties,
                                    body: body);
            }
    }
}
