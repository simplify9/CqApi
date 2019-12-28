
using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;

namespace SW.CqApi
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCqApi(this IServiceCollection services)
        {
            services.AddSingleton<ServiceDiscovery>();
            
            services.Scan(scan => scan
                .FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<IHandler>())
                .AsSelf().As<IHandler>().WithScopedLifetime());

            services.AddHttpContextAccessor();
            services.AddScoped<IRequestContext, RequestContext>();
        }
    }
}
