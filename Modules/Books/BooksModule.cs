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
        /**if (!services.Any(x => x.ServiceType == typeof(IConnectionFactory)))
            services.AddSingleton<IConnectionFactory>(provider =>
            {
                var connectionString = configuration.GetValue<string>("DatabaseConnectionString");
                return new SqlServerConnectionFactory(connectionString);
            });**/

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
                var book = service.GetById(new Models.BookId(id)).Result;
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
                messageService.SendMessage("Books requested", "books");
                return Results.Ok(books.Select(book => book.ToResponse()).ToList());
            }
        });

        endpoints.MapPost("/books", (PostBookRequest book) => {
            using(var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBookService>();
                var bookResult = service.Execute(book.ToCommand()).Result;
                return Results.Created($"/books/{bookResult.Id}", bookResult.ToResponse());
            }
        });

        endpoints.MapPut("/books", (PutBookRequest book) => {
            using(var scope = Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBookService>();
                var bookResult = service.Execute(book.ToCommand()).Result;
                return Results.NoContent();
            }
        });

        return endpoints;
    }
}