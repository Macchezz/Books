namespace test.Modules.Books;

public interface IBookMessagesService
{
    Task SendMessage(string message, string queue, string exchange);
}
public class BookMessagesService : IBookMessagesService
{
    public IBookMessageRepository _repository;

    public BookMessagesService(IBookMessageRepository repository)
    {
        _repository = repository;
    }

    public Task SendMessage(string message, string queue, string exchange) => _repository.Send(message, queue, exchange);
}
