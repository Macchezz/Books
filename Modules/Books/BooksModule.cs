using test.Infrastructure.Database;
using test.Infrastructure.Database.SqlServer;
using Autofac;
using Microsoft.AspNetCore.Mvc;
using test.Modules.Books.Models;

namespace test.Modules.Books;

public class BooksModule: IModule
{
    private static IContainer Container;
    public IServiceCollection RegisterModules(IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetValue<string>("DatabaseConnectionString");
        var builder = new ContainerBuilder();
        builder.RegisterType<BookService>().As<IBookService>();
        builder.RegisterType<SqlServerConnectionFactory>().As<IConnectionFactory>().WithParameter("connectionString", connectionString);
        builder.RegisterType<BookMessageRepository>().As<IBookMessageRepository>();
        builder.RegisterType<BookMessagesService>().As<IBookMessagesService>();
        builder.RegisterType<BooksRepository>().As<IBooksRepository>();
        Container = builder.Build();

        /*services.AddScoped<IBooksRepository, BooksRepository>();        
        services.AddScoped<IBookService, BookService>();*/
        return services;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/books/{id}", ([FromRoute] int id) => {
            using(var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBookService>();
                var messageService = scope.Resolve<IBookMessagesService>();
                var book = service.GetById(new Models.BookId(id)).Result;
                messageService.SendMessage($"Book with Id:{id} requested", "books.requests", "books");
                return book is null 
                ? Results.NotFound() 
                : Results.Ok(book.ToResponse());
            }
        });

        endpoints.MapGet("/books", () => {
            using(var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBookService>();
                var messageService = scope.Resolve<IBookMessagesService>();
                var books = service.GetAll().Result;
                messageService.SendMessage("All books requested", "books.requests", "books");
                return Results.Ok(books.Select(book => book.ToResponse()).ToList());
            }
        });

        endpoints.MapPost("/books", (PostBookRequest book) => {
            using(var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBookService>();
                var messageService = scope.Resolve<IBookMessagesService>();
                var bookResult = service.Execute(book.ToCommand()).Result;
                messageService.SendMessage($"Book with Id:{bookResult.Id} created", "books.creations", "books");
                return Results.Created($"/books/{bookResult.Id}", bookResult.ToResponse());
            }
        });

        endpoints.MapPut("/books", (PutBookRequest book) => {
            using(var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBookService>();
                var messageService = scope.Resolve<IBookMessagesService>();
                messageService.SendMessage($"Book with Id:{book.Id} updated", "books.updates", "books");
                var bookResult = service.Execute(book.ToCommand()).Result;
                return Results.NoContent();
            }
        });

        return endpoints;
    }
}