using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SignalRToolkit.Base;
using SignalRToolkit.Base.Attributes;
using SignalRToolkit.Filters;
using System;
using System.Linq;
using System.Reflection;

namespace SignalRToolkit.Extensions
{
    public static class HubServiceExtension
    {

        public static IServiceCollection RegisterSignalR(this IServiceCollection services, Type filterType = null)
        {
            services.AddSingleton(typeof(HubStorageHandler));

            if (filterType == null)

                services.AddTransient<IHubFilter, DefaultHubFilter>();
            else
                services.TryAddTransient(typeof(IHubFilter), filterType);


            return services;
        }

        public static void RegisterSignalRControllers<T>(this IEndpointRouteBuilder routeBuilder, string basePath = "/hubs") where T : BaseHub
        {
            RegisterWithCurrentAssembly<T>(routeBuilder, typeof(T).Assembly, basePath);
        }

        public static void RegisterSignalRControllers<T>(this IEndpointRouteBuilder routeBuilder, Assembly assembly, string basePath = "/hubs") where T : BaseHub
        {
            RegisterWithCurrentAssembly<T>(routeBuilder, assembly, basePath);
        }

        private static void RegisterWithCurrentAssembly<T>(IEndpointRouteBuilder routeBuilder, Assembly assembly, string basePath = "/hubs") where T : BaseHub
        {
            var store = routeBuilder.ServiceProvider.GetRequiredService<HubStorageHandler>();

            foreach (var type in assembly.GetTypes().Where(t => typeof(T).IsAssignableFrom(t) && t != typeof(T)))
            {
                if (type.GetMethods().Where(x => x.GetCustomAttributes(typeof(AuthorizeHubAttribute), false).Length > 0).FirstOrDefault() is MethodInfo method)
                {
                    AuthorizeHubAttribute authAttributeData = method.GetCustomAttribute(typeof(AuthorizeHubAttribute), true) as AuthorizeHubAttribute;

                    store.SetMethod(method.Name, authAttributeData.Roles);
                }
                if (type.GetCustomAttributes(typeof(RouteHubAttribute), false).Length > 0)
                {
                    var childAttr = type.GetCustomAttributes(typeof(RouteHubAttribute), false).FirstOrDefault() as RouteHubAttribute;

                    string fullPath = childAttr._path.StartsWith("/") || basePath.EndsWith("/") ? $"{basePath}{childAttr._path}" : $"{basePath}/{childAttr._path}";

                    InvokeMapHub(routeBuilder, fullPath, type);
                }
                else
                {
                    var fullPath = type.Name.EndsWith("HubController") ? type.Name.Replace("HubController", string.Empty) : type.Name;

                    InvokeMapHub(routeBuilder, fullPath, type);
                }
            }
        }

        private static void InvokeMapHub(IEndpointRouteBuilder routeBuilder, string fullPath, Type type)
        {
            var invokable = typeof(HubEndpointRouteBuilderExtensions)
                       .GetMethod("MapHub", new Type[] { typeof(IEndpointRouteBuilder), typeof(string) }).MakeGenericMethod(type);

            invokable.Invoke(null, new object[] { routeBuilder, fullPath });
        }
    }
}
