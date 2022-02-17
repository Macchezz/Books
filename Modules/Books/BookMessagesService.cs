namespace test.Modules.Books;

public interface IBookMessagesService
{
    void SendMessage(string message, string queue, string exchange);
}
public class BookMessagesService : IBookMessagesService
{
    public IBookMessageRepository _repository;

    public BookMessagesService(IBookMessageRepository repository)
    {
        _repository = repository;
    }

    public void SendMessage(string message, string queue, string exchange) => _repository.Send(message, queue, exchange);
}
