
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SW.CqApi.AuthOptions;
using SW.PrimitiveTypes;
using System;
using System.Reflection;

namespace SW.CqApi
{
    public static class IServiceCollectionExtensions
    {

        public static IServiceCollection AddCqApi(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies.Length == 0) assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            return services.AddCqApi(null, assemblies);
        }

        public static IServiceCollection AddCqApi(this IServiceCollection services, Action<CqApiOptions> configure, params Assembly[] assemblies)
        {

            var cqApiOptions = new CqApiOptions();
            if (configure != null) configure.Invoke(cqApiOptions);
            services.AddSingleton(cqApiOptions);

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

<<<<<<< HEAD
            services.AddHttpContextAccessor();
            services.AddScoped<RequestContext>();
=======
            //services.AddHttpContextAccessor();
            //services.AddScoped<IRequestContextProvider, RequestContextProvider>();
            //services.AddScoped<RequestContextManager>();
>>>>>>> origin/master

            services.AddRouting(options =>
            {
                options.ConstraintMap.Add("cqapiPrefix", typeof(CqapiPrefixRouteConstraint));
            });

            return services;
        }
    }
}
