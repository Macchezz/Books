namespace test.Modules.Books;

public interface IBookMessagesService
{
    void SendMessage(string message, string queue);
}
public class BookMessagesService : IBookMessagesService
{
    public IBookMessageRepository _repository;

    public BookMessagesService(IBookMessageRepository repository)
    {
        _repository = repository;
    }

    public void SendMessage(string message, string queue) => _repository.Send(message, queue);
}
