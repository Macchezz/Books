using System.Text;
using RabbitMQ.Client;

namespace test.Modules.Books;

public interface IBookMessageRepository
{
    void Send(string message, string queue);
}
public class BookMessageRepository : IBookMessageRepository
{
    public void Send(string message, string queue)
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

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                    routingKey: queue,
                                    basicProperties: properties,
                                    body: body);
            }
    }
}
