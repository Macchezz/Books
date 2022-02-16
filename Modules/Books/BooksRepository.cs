using test.Modules.Books.Models;
using test.Infrastructure.Database;
using Dapper;
using System.Text;
using RabbitMQ.Client;

namespace test.Modules.Books
{
    public interface IBooksRepository
    {
        Task<Book> GetById(BookId id);
        Task<Book> Save(Book book);
        Task<List<Book>> GetAll();
    }

    public class BooksRepository: IBooksRepository
    {
        private readonly test.Infrastructure.Database.IConnectionFactory connectionFactory;

        public BooksRepository(test.Infrastructure.Database.IConnectionFactory connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }
        public async Task<Book> GetById(BookId id)
        {
            using var connection = await connectionFactory.getReadyConnectionAsync();

            var data = await connection.QuerySingleOrDefaultAsync<BookEntity>($"Select Id, Title, Description FROM Books where Id = {id.Value}");
            return data is null ? null : new Book{
                        Id = new BookId(data.Id),
                        Title = data.Title,
                        Description = data.Description
                    };
        }

        public async Task<Book> Save(Book book)
        {
            using var connection = await connectionFactory.getReadyConnectionAsync();
            string sql = book.Id is null 
                ? "INSERT INTO Books (Title, Description) OUTPUT INSERTED.Id VALUES (@Title, @Description)" 
                : "UPDATE Books SET Title = @Title, Description = @Description WHERE Id = @Id; SELECT @Id;";
            var id = await connection.ExecuteScalarAsync<int>(sql, new{Title = book.Title, Description = book.Description, Id = book.Id?.Value ?? 0});
            return new Book{
                        Id = new BookId(id),
                        Title = book.Title,
                        Description = book.Description
                    };
        }

        public async Task<List<Book>> GetAll()
        {
            using var connection = await connectionFactory.getReadyConnectionAsync();
            string sql = "SELECT * FROM Books";
            var books = await connection.QueryAsync<BookEntity>(sql);

            return books.Select(book => new Book{Id = new BookId(book.Id),
                        Title = book.Title,
                        Description = book.Description}).ToList();

        }
    }

    public class BookEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}