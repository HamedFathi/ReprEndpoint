// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace TheReprEndpoint;

/// <summary>
/// Represents an endpoint with a request and a response.
/// </summary>
/// <typeparam name="TRequest">The request type (must be non-null).</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class ReprEndpoint<TRequest, TResponse> : ReprEndpointBase
    where TRequest : notnull
{
    /// <summary>
    /// Indicates whether the request should be bound from parameters instead of the body.
    /// </summary>
    public virtual bool RequestAsParameters => false;

    /// <summary>
    /// Handles the incoming request and returns a response asynchronously.
    /// </summary>
    /// <param name="request">The deserialized request object.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>A task that returns the response.</returns>
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);

    /// <summary>
    /// Maps a POST request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapPost(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPost(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPost(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a GET request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapGet(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapGet(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapGet(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a PUT request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapPut(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPut(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPut(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a DELETE request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapDelete(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapDelete(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapDelete(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a PATCH request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapPatch(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPatch(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPatch(pattern, HandleAsync);
    }
}