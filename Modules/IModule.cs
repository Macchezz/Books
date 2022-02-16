namespace test.Modules
{
    public interface IModule
    {
         IServiceCollection RegisterModules(IServiceCollection builder, IConfiguration configuration);
         IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints);
    }

    public static class ModuleExtensions
    {
        public static List<IModule> registeredModules = new List<IModule>();

        public static WebApplicationBuilder RegisterModules(this WebApplicationBuilder builder)
        {
            var modules = DiscoverModules();

            foreach(var module in modules)
            {
                module.RegisterModules(builder.Services, builder.Configuration);
                registeredModules.Add(module);
            }

            return builder;
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            foreach(var module in registeredModules)
            {
                module.MapEndpoints(app);
            }

            return app;
        }

        public static IEnumerable<IModule> DiscoverModules()
        {
            return typeof(IModule).Assembly
            .GetTypes()
            .Where(p => p.IsClass && p.IsAssignableTo(typeof(IModule)))
            .Select(Activator.CreateInstance)
            .Cast<IModule>();
        }
    }
}