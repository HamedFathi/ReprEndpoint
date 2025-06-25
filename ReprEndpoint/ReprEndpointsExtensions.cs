// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TheReprEndpoint;

/// <summary>
/// Extension methods for registering and mapping Repr endpoints in the application.
/// </summary>
public static class ReprEndpointsExtensions
{
    /// <summary>
    /// Registers all non-abstract endpoints derived from <see cref="ReprEndpointBase"/> found in the given assemblies.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceLifetime">The service lifetime for each endpoint.</param>
    /// <param name="assemblies">Optional assemblies to scan. If null or empty, current AppDomain assemblies are used.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
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
    /// Registers specific non-abstract endpoint types derived from <see cref="ReprEndpointBase"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceLifetime">The lifetime to use when registering the endpoints.</param>
    /// <param name="endpointTypes">The endpoint types to register.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if a type is not derived from <see cref="ReprEndpointBase"/> or is abstract.</exception>
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
    /// Maps all registered endpoints to the <see cref="WebApplication"/>, grouping them if a group prefix is specified.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> to map the endpoints to.</param>
    /// <returns>The modified <see cref="WebApplication"/>.</returns>
    public static WebApplication MapReprEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<ReprEndpointBase>();

        foreach (var endpoint in endpoints)
        {
            if (!string.IsNullOrWhiteSpace(endpoint.GroupPrefix))
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