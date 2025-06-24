// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace TheReprEndpoint;

/// <summary>
/// Base class for simple endpoints that return IResult responses without requiring request or response objects.
/// Useful for endpoints with minimal input/output requirements or when you need complete control over the response.
/// </summary>
public abstract class ReprEndpoint : ReprEndpointBase
{
    /// <summary>
    /// Handles the request asynchronously and returns an IResult.
    /// This method contains the core business logic for the endpoint.
    /// </summary>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing an IResult response.</returns>
    public abstract Task<IResult> HandleAsync(CancellationToken ct = default);

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