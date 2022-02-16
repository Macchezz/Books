namespace test.Modules.Books.Models
{
    public static class MappingExtensions
    {
        public static CreateBookCommand ToCommand(this PostBookRequest request) => new CreateBookCommand(Title: request.Title, Description: request.Description);
        public static BookResponse ToResponse(this Book resource) => new BookResponse{Title = resource.Title, Description = resource.Description, Id = resource.Id.Value};
        public static UpdateBookCommand ToCommand(this PutBookRequest request) => new UpdateBookCommand(Title: request.Title, Description: request.Description, Id: new BookId(request.Id));
    }
}