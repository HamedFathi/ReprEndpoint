// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

/// <summary>
/// Base class for all endpoint types with route mapping and optional group configuration.
/// </summary>
public abstract class ReprEndpointBase
{
    /// <summary>
    /// Optional prefix for the route group. If not set, the endpoint will not be grouped.
    /// </summary>
    public virtual string? GroupPrefix => null;

    /// <summary>
    /// Optional delegate to configure the route group.
    /// </summary>
    public virtual Action<RouteGroupBuilder>? ConfigureGroup => null;

    /// <summary>
    /// Abstract method to map the endpoint to the route builder.
    /// </summary>
    /// <param name="routes">The route builder to which the endpoint is added.</param>
    public abstract void MapEndpoint(IEndpointRouteBuilder routes);
}