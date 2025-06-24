// ReSharper disable UnusedMember.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

/// <summary>
/// Base class for endpoints that handle request objects and return IResult responses.
/// Useful when you need fine-grained control over the HTTP response or want to return different response types.
/// </summary>
/// <typeparam name="TRequest">The type of the request object. Must be non-null.</typeparam>
public abstract class ReprRequestEndpoint<TRequest> : ReprEndpointBase
    where TRequest : notnull
{
    /// <summary>
    /// Gets a value indicating whether the request should be bound as parameters instead of from the request body.
    /// When true, request properties are bound from query string, route values, or form data.
    /// When false, the request is bound from the request body (typically JSON).
    /// </summary>
    /// <value>True if request should be bound as parameters; otherwise, false. Default is false.</value>
    public virtual bool RequestAsParameters => false;

    /// <summary>
    /// Handles the incoming request asynchronously and returns an IResult.
    /// This method contains the core business logic for the endpoint.
    /// </summary>
    /// <param name="request">The strongly-typed request object.</param>
    /// <param name="ct">The cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation, containing an IResult response.</returns>
    public abstract Task<IResult> HandleAsync(TRequest request, CancellationToken ct = default);

    /// <summary>
    /// Maps a POST endpoint with the specified route pattern.
    /// The request binding behavior is determined by the <see cref="RequestAsParameters"/> property.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the POST endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapPost(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPost(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPost(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a GET endpoint with the specified route pattern.
    /// The request binding behavior is determined by the <see cref="RequestAsParameters"/> property.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the GET endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapGet(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapGet(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapGet(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a PUT endpoint with the specified route pattern.
    /// The request binding behavior is determined by the <see cref="RequestAsParameters"/> property.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the PUT endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapPut(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPut(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPut(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a DELETE endpoint with the specified route pattern.
    /// The request binding behavior is determined by the <see cref="RequestAsParameters"/> property.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the DELETE endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapDelete(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapDelete(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapDelete(pattern, HandleAsync);
    }

    /// <summary>
    /// Maps a PATCH endpoint with the specified route pattern.
    /// The request binding behavior is determined by the <see cref="RequestAsParameters"/> property.
    /// </summary>
    /// <param name="routes">The endpoint route builder to map the PATCH endpoint to.</param>
    /// <param name="pattern">The route pattern for the endpoint.</param>
    /// <returns>A RouteHandlerBuilder for further configuration of the endpoint.</returns>
    protected RouteHandlerBuilder MapPatch(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPatch(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPatch(pattern, HandleAsync);
    }
}