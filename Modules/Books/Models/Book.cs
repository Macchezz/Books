namespace test.Modules.Books.Models
{
    public record BookId(int Value)
    {
        public override string ToString() => Value.ToString();
    }

    public class Book
    {
        public BookId Id {get; set;}
        public string Title {get; set;}
        public string Description {get; set;}
        
    }

    public class PostBookRequest
    {
        public string Title {get; set;}
        public string Description {get; set;}
    }

    public class BookResponse
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public string Description {get; set;}
    }

    public class PutBookRequest
    {
        public int Id {get; set;}
        public string Title {get; set;}
        public string Description {get; set;}
    }
}