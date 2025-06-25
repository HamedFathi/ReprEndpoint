// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

/// <summary>
/// Represents an endpoint with a response and no request.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public abstract class ReprResponseEndpoint<TResponse> : ReprEndpointBase
{
    /// <summary>
    /// Handles the request asynchronously and returns the response.
    /// </summary>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>The response.</returns>
    public abstract Task<TResponse> HandleAsync(CancellationToken ct = default);

    /// <summary>
    /// Maps a POST request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapPost(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPost(pattern, HandleAsync);

    /// <summary>
    /// Maps a GET request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapGet(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapGet(pattern, HandleAsync);

    /// <summary>
    /// Maps a PUT request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapPut(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPut(pattern, HandleAsync);

    /// <summary>
    /// Maps a DELETE request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapDelete(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapDelete(pattern, HandleAsync);

    /// <summary>
    /// Maps a PATCH request to this endpoint.
    /// </summary>
    /// <param name="routes">The route builder.</param>
    /// <param name="pattern">The route pattern.</param>
    /// <returns>A <see cref="RouteHandlerBuilder"/> instance.</returns>
    protected RouteHandlerBuilder MapPatch(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPatch(pattern, HandleAsync);
}