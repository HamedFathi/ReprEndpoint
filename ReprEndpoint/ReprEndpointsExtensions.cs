// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TheReprEndpoint;

/// <summary>
/// Provides extension methods for registering and mapping Repr endpoints in the dependency injection container and routing system.
/// </summary>
public static class ReprEndpointsExtensions
{
    /// <summary>
    /// Registers all Repr endpoints found in the specified assemblies with the dependency injection container.
    /// If no assemblies are provided, scans all assemblies in the current AppDomain.
    /// </summary>
    /// <param name="services">The service collection to register endpoints with.</param>
    /// <param name="serviceLifetime">The service lifetime for the registered endpoints. Default is Transient.</param>
    /// <param name="assemblies">The assemblies to scan for endpoint types. If null or empty, scans all loaded assemblies.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddReprEndpoints(this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        params Assembly[]? assemblies)
    {
        var assembliesToScan = assemblies?.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
        var endpointTypes = assembliesToScan
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ReprEndpointBase)) && !type.IsAbstract);

        foreach (var type in endpointTypes)
        {
            services.Add(new ServiceDescriptor(type, type, serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(ReprEndpointBase), type, serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Registers specific Repr endpoint types with the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to register endpoints with.</param>
    /// <param name="serviceLifetime">The service lifetime for the registered endpoints.</param>
    /// <param name="endpointTypes">The specific endpoint types to register. Must inherit from ReprEndpointBase and not be abstract.</param>
    /// <returns>The service collection for method chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when an endpoint type doesn't inherit from ReprEndpointBase or is abstract.</exception>
    public static IServiceCollection AddReprEndpoints(this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        params Type[]? endpointTypes)
    {
        if (endpointTypes == null || endpointTypes.Length == 0)
            return services;

        foreach (var type in endpointTypes)
        {
            if (!type.IsSubclassOf(typeof(ReprEndpointBase)))
                throw new ArgumentException($"Type {type.Name} must inherit from {nameof(ReprEndpointBase)}", nameof(endpointTypes));
            if (type.IsAbstract)
                throw new ArgumentException($"Type {type.Name} cannot be abstract", nameof(endpointTypes));
        }

        foreach (var type in endpointTypes)
        {
            services.Add(new ServiceDescriptor(type, type, serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(ReprEndpointBase), type, serviceLifetime));
        }

        return services;
    }

    /// <summary>
    /// Maps all registered Repr endpoints to the web application's routing system.
    /// Endpoints with a GroupPrefix will be mapped under route groups, while others are mapped directly to the application.
    /// </summary>
    /// <param name="app">The web application to map endpoints to.</param>
    /// <returns>The web application for method chaining.</returns>
    public static WebApplication MapReprEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<ReprEndpointBase>();

        foreach (var endpoint in endpoints)
        {
            if (!string.IsNullOrEmpty(endpoint.GroupPrefix))
            {
                var group = app.MapGroup(endpoint.GroupPrefix);
                endpoint.ConfigureGroup?.Invoke(group);
                endpoint.MapEndpoint(group);
            }
            else
            {
                endpoint.MapEndpoint(app);
            }
        }

        return app;
    }
}