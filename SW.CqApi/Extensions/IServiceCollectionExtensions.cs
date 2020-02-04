
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SW.PrimitiveTypes;
using System.Reflection;

namespace SW.CqApi
{
    public static class IServiceCollectionExtensions
    {
        public static void AddCqApi(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<ServiceDiscovery>();

            if (assemblies.Length == 0) assemblies = new Assembly[] { Assembly.GetCallingAssembly() };

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IHandler>())
                .AsSelf().As<IHandler>().WithScopedLifetime());

            services.Scan(scan => scan
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IValidator>())
                .AsImplementedInterfaces().WithTransientLifetime());

            services.AddHttpContextAccessor();
            services.AddScoped<IRequestContext, RequestContext>();
            services.AddScoped<RequestContextManager>();
        }
    }
}
