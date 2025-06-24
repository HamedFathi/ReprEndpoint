// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

/// <summary>
/// Base class for endpoints that return a strongly-typed response without requiring a request object.
/// Useful for endpoints that don't need input parameters or retrieve all necessary data from the service layer.
/// </summary>
/// <typeparam name="TResponse">The type of the response object.</typeparam>
public abstract class ReprResponseEndpoint<TResponse> : ReprEndpointBase
{
    /// <summary>
    /// Handles the request asynchronously and returns the response.
    /// This method contains the core business logic for the endpoint.
    /// </summary>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response object.</returns>
    public abstract Task<TResponse> HandleAsync(CancellationToken ct = default);

    /// <summary>
    /// Maps a POST endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the POST endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapPost(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPost(pattern, HandleAsync);

    /// <summary>
    /// Maps a GET endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the GET endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapGet(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapGet(pattern, HandleAsync);

    /// <summary>
    /// Maps a PUT endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the PUT endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapPut(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPut(pattern, HandleAsync);

    /// <summary>
    /// Maps a DELETE endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the DELETE endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapDelete(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapDelete(pattern, HandleAsync);

    /// <summary>
    /// Maps a PATCH endpoint with the specified route pattern.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the PATCH endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapPatch(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPatch(pattern, HandleAsync);
}