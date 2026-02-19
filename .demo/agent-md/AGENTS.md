# AGENTS.md

This file provides guidance for AI coding agents working on the MythAPI project.

## Project Overview

MythAPI is a .NET 8 Web API that provides information about gods and mythologies from various cultures. The API is built using ASP.NET Core Minimal APIs with Entity Framework Core for database access.

**Key Technologies:**
- .NET 8
- ASP.NET Core Minimal APIs
- Entity Framework Core
- SQLite/SQL Server (configurable)
- xUnit for testing

**Project Structure:**
- `src/` - Main API source code
  - `Endpoints/v1/` - API endpoint definitions
  - `Gods/` - God-related business logic and repositories
  - `Mythologies/` - Mythology-related business logic
  - `Common/` - Shared utilities, database context, and models
  - `Migrations/` - EF Core database migrations
- `tests/` - Test projects
  - `UnitTests/` - Unit tests
  - `IntegrationTests/` - Integration tests with in-memory database

## Setup Commands

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the API locally
dotnet run --project src/MythApi.csproj

# Run with watch mode (auto-reload on changes)
dotnet watch run --project src/MythApi.csproj

# Run all tests
dotnet test

# Run only unit tests
dotnet test tests/UnitTests/UnitTests.csproj

# Run only integration tests
dotnet test tests/IntegrationTests/IntegrationTests.csproj

# Create a new migration
dotnet ef migrations add <MigrationName> --project src/MythApi.csproj

# Update database
dotnet ef database update --project src/MythApi.csproj
```

## Build and Test Instructions

### Using VS Code Tasks
Three tasks are available in `.vscode/tasks.json`:
- `build` - Builds the project
- `publish` - Publishes the project for deployment
- `watch` - Runs the project with hot reload

### Running Tests
- Always run tests before committing: `dotnet test`
- Tests should pass with zero failures before creating a PR
- Add or update tests for any code changes
- Integration tests use `CustomWebApplicationFactory` with in-memory database

### Docker
```bash
# Build and run with Docker Compose
docker compose up --build

# The API will be available at http://localhost:5000
```

## Code Style Guidelines

### C# Conventions
- Follow standard C# naming conventions (PascalCase for public members, camelCase for private)
- Use `var` when the type is obvious from the right side
- Prefer expression-bodied members for simple one-liners
- Use async/await consistently for I/O operations
- Keep methods focused and single-purpose

### API Endpoint Patterns
- Endpoints are defined as static methods in `src/Endpoints/v1/`
- Use method groups with `MapGroup()` for versioned routes (e.g., `/api/v1/gods`)
- Inject dependencies through method parameters
- Follow RESTful conventions:
  - GET for retrieving data
  - POST for creating/updating (batch operations)
  - Use route parameters for IDs: `{id}`
  - Use query parameters for optional filters: `[FromQuery]`

### Repository Pattern
- All data access goes through repository interfaces in `Interfaces/` folders
- Implementations in `DBRepositories/` folders
- Use async methods for all database operations
- Return `Task<IList<T>>` or `Task<List<T>>` for collections
- Load related entities explicitly when needed using `_context.Entry(entity).Collection(x => x.Property).Load()`

### Database Models
- Database models are in `src/Common/Database/Models/`
- Use EF Core conventions for relationships
- Keep models simple DTOs without business logic

## Testing Guidelines

### Unit Tests
- Located in `tests/UnitTests/`
- Test endpoint methods in isolation
- Mock `IGodRepository` and other dependencies
- Use xUnit's `[Fact]` and `[Theory]` attributes

### Integration Tests
- Located in `tests/IntegrationTests/`
- Use `CustomWebApplicationFactory` for in-memory testing
- Test full HTTP request/response cycle
- Verify status codes and response content
- Tests should be independent and not rely on execution order

## Common Tasks

### Adding a New Endpoint
1. Define the endpoint in `src/Endpoints/v1/<EntityName>.cs`
2. Add the method signature to the repository interface
3. Implement the method in the database repository
4. Add tests in both UnitTests and IntegrationTests
5. Run tests to verify: `dotnet test`

### Adding a New Entity
1. Create the model in `src/Common/Database/Models/`
2. Add DbSet to `AppDbContext.cs`
3. Create a migration: `dotnet ef migrations add Add<EntityName>`
4. Update the database: `dotnet ef database update`
5. Create repository interface and implementation
6. Create endpoint definitions
7. Add tests

### Modifying Database Schema
1. Update the model in `src/Common/Database/Models/`
2. Create migration: `dotnet ef migrations add <DescriptiveName> --project src/MythApi.csproj`
3. Review the generated migration in `src/Migrations/`
4. Apply migration: `dotnet ef database update --project src/MythApi.csproj`
5. Update any affected repository methods
6. Update tests to reflect schema changes

## API Endpoints

### Current Endpoints (v1)

**Gods:**
- `GET /api/v1/gods` - Get all gods
- `GET /api/v1/gods/{id}` - Get god by ID
- `GET /api/v1/gods/search/{name}?includeAliases=false` - Search gods by name
- `GET /api/v1/gods/mythology/{mythologyName}` - Get gods by mythology
- `POST /api/v1/gods` - Add or update gods (batch)

**Mythologies:**
- Endpoints defined in `src/Endpoints/v1/Mythologies.cs`

## Configuration

### appsettings.json
- Connection strings for different environments
- Logging configuration
- CORS policies if needed

### Environment-Specific Settings
- `appsettings.Development.json` - Development overrides
- `appsettings.json` - Base configuration

## Security Considerations

- Sanitize user input in search queries to prevent SQL injection
- The current `GetGodByNameAsync` uses raw SQL - be cautious with user input
- Consider using parameterized queries or LINQ for all database operations
- Validate input parameters before processing

## PR and Commit Guidelines

### Commit Messages
- Use clear, descriptive commit messages
- Format: `<type>: <description>`
- Types: `feat`, `fix`, `refactor`, `test`, `docs`, `chore`
- Example: `feat: add GetGodsByMythology endpoint`

### Pull Requests
- Ensure all tests pass: `dotnet test`
- Build must succeed: `dotnet build`
- Update tests for any changed functionality
- Keep PRs focused on a single feature or fix
- Reference related issues if applicable

## Known Issues and Considerations

- The `GetGodByNameAsync` method uses raw SQL which could be vulnerable to SQL injection
- Consider implementing pagination for endpoints returning large collections
- API versioning is in place (v1) - maintain backward compatibility or increment version
- Error handling could be enhanced with proper HTTP status codes and problem details

## Useful Commands Reference

```bash
# Clean build artifacts
dotnet clean

# List available migrations
dotnet ef migrations list --project src/MythApi.csproj

# Remove last migration (if not applied)
dotnet ef migrations remove --project src/MythApi.csproj

# Generate SQL script from migrations
dotnet ef migrations script --project src/MythApi.csproj

# Format code (if dotnet-format is installed)
dotnet format

# Watch tests (requires dotnet-watch)
dotnet watch test
```
