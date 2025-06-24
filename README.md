# ReprEndpoint Library User Guide

## The REPR Pattern

The **REPR (Request-Endpoint-Response) pattern** is a modern architectural approach for building ASP.NET Core APIs that promotes clean, maintainable, and testable code. Unlike traditional controller-based architectures, REPR organizes your API around individual endpoint classes, each representing a single operation.

### Why Use the REPR Pattern?

* **Single Responsibility Principle**: Each endpoint class handles exactly one operation, making your code more focused and easier to understand.
* **Better Testability**: Individual endpoints can be unit tested in isolation without the complexity of controller dependencies.
* **Improved Organization**: Related logic is contained within a single class, reducing the cognitive load when working with complex APIs.
* **Enhanced Maintainability**: Changes to one endpoint don't affect others, reducing the risk of introducing bugs.
* **Cleaner Dependency Injection**: Each endpoint can have its own specific dependencies without bloating a shared controller.
* **Type Safety**: Strong typing for requests and responses with compile-time validation.

## Getting Started

### Installation

Add the ReprEndpoint library to your project:

```xml
<PackageReference Include="ReprEndpoint" Version="1.0.0" />
```

Or visit the NuGet package page: [https://www.nuget.org/packages/ReprEndpoint](https://www.nuget.org/packages/ReprEndpoint)

You can also install it via the Visual Studio Package Manager UI by searching for "ReprEndpoint".

### Basic Setup

Configure your ASP.NET Core application to use ReprEndpoint:

```csharp
using TheReprEndpoint;

var builder = WebApplication.CreateBuilder(args);

// Register all endpoints from the current assembly
builder.Services.AddReprEndpoints();

// Add other services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map all registered endpoints
app.MapReprEndpoints();

app.Run();
```

## Base Classes Overview

The ReprEndpoint library provides four base classes to suit different endpoint scenarios:

### 1. `ReprEndpoint<TRequest, TResponse>`

Use this when your endpoint needs both a strongly-typed request and response:

```csharp
public class CreateUserEndpoint : ReprEndpoint<CreateUserRequest, UserResponse>
{
    private readonly IUserService _userService;

    public CreateUserEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<UserResponse> HandleAsync(CreateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userService.CreateUserAsync(request.Name, request.Email, ct);
        return new UserResponse 
        { 
            Id = user.Id, 
            Name = user.Name, 
            Email = user.Email 
        };
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapPost(routes, "/users")
            .WithName("CreateUser")
            .WithOpenApi();
    }
}

public record CreateUserRequest(string Name, string Email);
public record UserResponse(int Id, string Name, string Email);
```

### 2. `ReprRequestEndpoint<TRequest>`

Use this when you need a strongly-typed request but want to return an `IResult` for flexible response handling:

```csharp
public class UpdateUserEndpoint : ReprRequestEndpoint<UpdateUserRequest>
{
    private readonly IUserService _userService;

    public UpdateUserEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<IResult> HandleAsync(UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await _userService.GetUserAsync(request.Id, ct);
        if (user == null)
            return Results.NotFound($"User with ID {request.Id} not found");

        await _userService.UpdateUserAsync(request.Id, request.Name, request.Email, ct);
        return Results.NoContent();
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapPut(routes, "/users/{id}")
            .WithName("UpdateUser")
            .WithOpenApi();
    }
}

public record UpdateUserRequest(int Id, string Name, string Email);
```

### 3. `ReprResponseEndpoint<TResponse>`

Use this for endpoints that don't require input parameters but return a strongly-typed response:

```csharp
public class GetAllUsersEndpoint : ReprResponseEndpoint<List<UserResponse>>
{
    private readonly IUserService _userService;

    public GetAllUsersEndpoint(IUserService userService)
    {
        _userService = userService;
    }

    public override async Task<List<UserResponse>> HandleAsync(CancellationToken ct = default)
    {
        var users = await _userService.GetAllUsersAsync(ct);
        return users.Select(u => new UserResponse(u.Id, u.Name, u.Email)).ToList();
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapGet(routes, "/users")
            .WithName("GetAllUsers")
            .WithOpenApi();
    }
}
```

### 4. `ReprEndpoint`

Use this for simple endpoints that don't need strongly-typed requests or responses:

```csharp
public class HealthCheckEndpoint : ReprEndpoint
{
    private readonly ILogger<HealthCheckEndpoint> _logger;

    public HealthCheckEndpoint(ILogger<HealthCheckEndpoint> logger)
    {
        _logger = logger;
    }

    public override Task<IResult> HandleAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Health check requested");
        return Task.FromResult(Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapGet(routes, "/health")
            .WithName("HealthCheck")
            .WithOpenApi();
    }
}
```

## Request/Response Binding

### Request Body Binding (Default)

By default, requests are bound from the request body (typically JSON):

```csharp
public class CreateProductEndpoint : ReprEndpoint<CreateProductRequest, ProductResponse>
{
    // RequestAsParameters is false by default
    public override bool RequestAsParameters => false;

    public override async Task<ProductResponse> HandleAsync(CreateProductRequest request, CancellationToken ct)
    {
        // request is bound from JSON body
        // POST /products
        // Body: { "name": "Laptop", "price": 999.99 }
    }
}
```

### Parameter Binding

Override `RequestAsParameters` to bind from query string, route values, or form data:

```csharp
public class GetUserEndpoint : ReprEndpoint<GetUserRequest, UserResponse>
{
    //Apply [AsParameters] on your request.
    public override bool RequestAsParameters => true;

    public override async Task<UserResponse> HandleAsync(GetUserRequest request, CancellationToken ct)
    {
        // request is bound from route and query parameters
        // GET /users/123?includeDetails=true
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapGet(routes, "/users/{id}")
            .WithName("GetUser")
            .WithOpenApi();
    }
}

public record GetUserRequest(int Id, bool IncludeDetails = false);
```

## Endpoint Grouping and Configuration

### Route Groups

Group related endpoints under a common prefix:

```csharp
public class GetUserProfileEndpoint : ReprResponseEndpoint<UserProfile>
{
    public override string? GroupPrefix => "/api/v1/users";

    public override Action<RouteGroupBuilder>? ConfigureGroup => group =>
    {
        group.RequireAuthorization()
             .WithTags("Users")
             .WithOpenApi();
    };

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapGet(routes, "/{id}/profile")
            .WithName("GetUserProfile");
    }
}
```

This creates the endpoint at `/api/v1/users/{id}/profile` with authorization requirements.

### Advanced Group Configuration

```csharp
public class AdminUserEndpoint : ReprEndpoint<AdminRequest, AdminResponse>
{
    public override string? GroupPrefix => "/api/admin";

    public override Action<RouteGroupBuilder>? ConfigureGroup => group =>
    {
        group.RequireAuthorization("AdminPolicy")
             .AddEndpointFilter<AdminLoggingFilter>()
             .WithTags("Administration")
             .WithOpenApi();
    };
}
```

## API Versioning Support

The library integrates seamlessly with ASP.NET Core API versioning:

```csharp
public class GetWeatherForecastV1Endpoint : ReprResponseEndpoint<WeatherForecast[]>
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<GetWeatherForecastV1Endpoint> _logger;

    public GetWeatherForecastV1Endpoint(ILogger<GetWeatherForecastV1Endpoint> logger)
    {
        _logger = logger;
    }

    public override Task<WeatherForecast[]> HandleAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Generating weather forecast for 5 days (V1)");

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();

        return Task.FromResult(forecast);
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        var versionSet = routes.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .Build();

        MapGet(routes, "/v{version:apiVersion}/weatherforecast")
            .WithName("GetWeatherForecastV1")
            .WithApiVersionSet(versionSet)
            .WithOpenApi();
    }
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

### Versioning Setup

```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("version"),
        new HeaderApiVersionReader("X-Version"),
        new UrlSegmentApiVersionReader()
    );
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
```

## Dependency Injection Integration

### Automatic Registration

Register all endpoints from assemblies:

```csharp
// Register from current assembly with default lifetime (Transient)
builder.Services.AddReprEndpoints();

// Register from specific assemblies
builder.Services.AddReprEndpoints(ServiceLifetime.Scoped, typeof(UserEndpoint).Assembly);

// Register specific endpoint types
builder.Services.AddReprEndpoints(ServiceLifetime.Singleton, typeof(HealthCheckEndpoint));
```

### Service Lifetimes

Choose appropriate service lifetimes based on your needs:

- **Transient** (default): New instance for each request
- **Scoped**: One instance per HTTP request
- **Singleton**: Single instance for the application lifetime

### Endpoint Dependencies

Inject services into your endpoints:

```csharp
public class ProcessOrderEndpoint : ReprEndpoint<ProcessOrderRequest, OrderResult>
{
    private readonly IOrderService _orderService;
    private readonly IPaymentService _paymentService;
    private readonly IInventoryService _inventoryService;
    private readonly ILogger<ProcessOrderEndpoint> _logger;

    public ProcessOrderEndpoint(
        IOrderService orderService,
        IPaymentService paymentService,
        IInventoryService inventoryService,
        ILogger<ProcessOrderEndpoint> logger)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _inventoryService = inventoryService;
        _logger = logger;
    }

    public override async Task<OrderResult> HandleAsync(ProcessOrderRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Processing order {OrderId}", request.OrderId);

        // Check inventory
        var available = await _inventoryService.CheckAvailabilityAsync(request.Items, ct);
        if (!available)
            throw new InvalidOperationException("Insufficient inventory");

        // Process payment
        var paymentResult = await _paymentService.ProcessPaymentAsync(request.Payment, ct);
        if (!paymentResult.Success)
            throw new InvalidOperationException("Payment failed");

        // Create order
        var order = await _orderService.CreateOrderAsync(request, ct);
        
        _logger.LogInformation("Order {OrderId} processed successfully", order.Id);
        
        return new OrderResult(order.Id, order.Status, order.Total);
    }

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapPost(routes, "/orders/process")
            .WithName("ProcessOrder")
            .RequireAuthorization()
            .WithOpenApi();
    }
}
```

## Contributing

We welcome contributions to make `ReprEndpoint` even better! Here are some ways you can help:

### 🌟 **Star this repository** if you find it useful!

Your star helps others discover this library and motivates continued development.

### 🔧 **Pull Requests Welcome**

We're open to pull requests!

Please feel free to fork the repository and submit a pull request. For larger changes, consider opening an issue first to discuss your approach.

### 📝 **Reporting Issues**

Found a bug or have a suggestion? Please open an issue with:

- A clear description of the problem or enhancement
- Steps to reproduce (for bugs)
- Sample code demonstrating the issue
- Expected vs actual behavior
