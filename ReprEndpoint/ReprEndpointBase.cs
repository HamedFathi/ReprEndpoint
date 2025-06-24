// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

/// <summary>
/// Base class for all endpoint representations in the Repr endpoint pattern.
/// Provides common functionality for endpoint configuration and mapping.
/// </summary>
public abstract class ReprEndpointBase
{
    /// <summary>
    /// Gets the optional group prefix for organizing related endpoints under a common route prefix.
    /// </summary>
    /// <value>The route prefix for the endpoint group, or null if no grouping is desired.</value>
    public virtual string? GroupPrefix => null;

    /// <summary>
    /// Gets an optional configuration action to customize the route group builder.
    /// This allows for adding middleware, authentication, or other group-level configurations.
    /// </summary>
    /// <value>An action that configures the RouteGroupBuilder, or null if no configuration is needed.</value>
    public virtual Action<RouteGroupBuilder>? ConfigureGroup => null;

    /// <summary>
    /// Maps the endpoint to the specified route builder.
    /// This method must be implemented by derived classes to define the specific endpoint mapping.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the endpoint to.</param>
    public abstract void MapEndpoint(IEndpointRouteBuilder routes);
}