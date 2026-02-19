# MythAPI - AI Agent Guidelines

## Project Overview

.NET 8 Web API for mythology and god information using ASP.NET Core Minimal APIs, Entity Framework Core, and domain-driven folder structure.

**Tech Stack:** .NET 8, EF Core, SQLite/PostgreSQL, Swagger, Serilog, xUnit/NUnit, Moq

## Architecture

### Domain-Driven Structure
Unlike typical layered architectures, this project organizes by domain (not by layers):
- `src/Gods/` - All god-related code (endpoints, repositories, interfaces, models)
- `src/Mythologies/` - All mythology-related code
- `src/Common/Database/` - Shared database context and models
- `src/Endpoints/v1/` - API endpoint registration methods

Reference: [Program.cs](src/Program.cs) for dependency registration pattern

### Database Flexibility
Three database modes configured in [Program.cs](src/Program.cs#L35-L82):
1. **In-memory** - Flag: `--in-memory-database`
2. **SQLite** - Development default, auto-creates `mythapi.db`
3. **PostgreSQL** - Production, uses Azure Key Vault for connection strings

All modes use [DatabaseInitializer](src/Common/Database/DatabaseInitializer.cs) for seeding.

### Endpoint Pattern
Endpoints follow Minimal API convention with static extension methods:
```csharp
// src/Endpoints/v1/Gods.cs
public static void RegisterGodEndpoints(this IEndpointRouteBuilder endpoints) {
    var gods = endpoints.MapGroup("/api/v1/gods");
    gods.MapGet("", GetAllGods);
    gods.MapGet("{id}", (int id, IGodRepository repo) => repo.GetGodAsync(...));
}
```
- Group routes with `MapGroup()` for versioning
- Use method parameters for dependency injection
- Register in [Program.cs](src/Program.cs#L111-L112) with `app.RegisterGodEndpoints()`

### Repository Pattern
- Interfaces in `<Domain>/Interfaces/` (e.g., [IGodRepository](src/Gods/Interfaces/IGodRepository.cs))
- Implementations in `<Domain>/DBRepositories/` (e.g., [GodRepository](src/Gods/DBRepositories/GodRepository.cs))
- All methods return `Task<T>` for async operations
- Load related entities explicitly: `_context.Entry(god).Collection(x => x.Aliases).Load()`
- Register as scoped services: `builder.Services.AddScoped<IGodRepository, GodRepository>()`

## Build and Test

```bash
# Run API (Development mode uses SQLite)
dotnet run --project src/MythApi.csproj

# Watch mode with hot reload
dotnet watch run --project src/MythApi.csproj

# Run all tests (ALWAYS before committing)
dotnet test

# Run specific test projects
dotnet test tests/UnitTests/UnitTests.csproj
dotnet test tests/IntegrationTests/IntegrationTests.csproj

# Create migration
dotnet ef migrations add MigrationName --project src/MythApi.csproj

# Update database
dotnet ef database update --project src/MythApi.csproj

# Docker
docker compose up --build
```

VS Code tasks available: `build`, `publish`, `watch`

## Project Conventions

### Testing Strategy
**Unit Tests** ([GodEndpointsTests.cs](tests/UnitTests/GodEndpointsTests.cs))
- Use NUnit + Moq
- Test endpoint static methods directly
- Mock repository dependencies: `Mock<IGodRepository>`

**Integration Tests** ([GodsEndpointTests.cs](tests/IntegrationTests/GodsEndpointTests.cs))
- Use [CustomWebApplicationFactory](tests/IntegrationTests/CustomWebApplicationFactory.cs) with SQLite
- Test full HTTP request/response cycle
- Access endpoints via `HttpClient`: `await _httpClient.GetAsync("/api/v1/gods")`
- Tests are independent, order doesn't matter

### Entity Framework Conventions
- Models in [src/Common/Database/Models/](src/Common/Database/Models/)
- DbSets in [AppDbContext](src/Common/Database/AppDBContext.cs)
- Configure relationships in `OnModelCreating()`
- Use `ToTable()` for explicit table names
- Migrations auto-generated in `src/Migrations/`

### Code Style
- Use `var` when type is obvious
- Async/await for all I/O operations
- Expression-bodied members for simple methods
- PascalCase for public members, camelCase for private/parameters
- Keep endpoint methods focused - complex logic goes in repositories

## Security

⚠️ **Known Issue:** [GetGodByNameAsync](src/Gods/DBRepositories/GodRepository.cs#L56) uses `FromSqlRaw()` with string interpolation - vulnerable to SQL injection. When fixing, use parameterized queries or LINQ.

## Integration Points

- **Azure Key Vault** - Production configuration (connection strings stored as secrets)
- **Azure Identity** - Default credential for Key Vault access
- **Serilog** - Structured logging to console, configured in [Program.cs](src/Program.cs#L17-L23)
- **Swagger** - Available at `/swagger` when running

## Adding Features

**New Endpoint:**
1. Add method to `src/Endpoints/v1/<Entity>.cs`
2. Add method signature to repository interface
3. Implement in DB repository
4. Register in endpoint group
5. Add unit test (mock repository)
6. Add integration test (HTTP client)

**New Entity:**
1. Create model in `src/Common/Database/Models/`
2. Add DbSet to [AppDbContext](src/Common/Database/AppDBContext.cs)
3. Create migration: `dotnet ef migrations add AddEntity`
4. Create repository interface and implementation following existing pattern
5. Create endpoints file in `src/Endpoints/v1/`
6. Register endpoints in [Program.cs](src/Program.cs)
7. Add comprehensive tests
