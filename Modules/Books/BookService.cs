using test.Modules.Books.Models;

namespace test.Modules.Books
{
    public interface IBookService
    {
        Task<Book> GetById(BookId id);
        Task<Book> Execute(CreateBookCommand command);
        Task<Book> Execute(UpdateBookCommand command);
        Task<List<Book>> GetAll();
    }
    public class BookService: IBookService
    {
        public IBooksRepository repository;

        public BookService(IBooksRepository repository)
        {
            this.repository = repository;
        }
        public Task<Book> GetById(BookId id) => repository.GetById(id);
        public Task<Book> Execute(CreateBookCommand command) => repository.Save(new Book{Id = null, Title = command.Title, Description = command.Description});
        public Task<Book> Execute(UpdateBookCommand command) => repository.Save(new Book{Id = command.Id, Title = command.Title, Description = command.Description});
        public Task<List<Book>> GetAll() => repository.GetAll();
    }
}