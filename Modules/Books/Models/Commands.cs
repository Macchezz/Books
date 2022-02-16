namespace test.Modules.Books.Models;
public record CreateBookCommand(string Title, string Description);

public record UpdateBookCommand(BookId Id, string Title, string Description);