using Microsoft.EntityFrameworkCore;
using MythApi.Gods.Interfaces;
using MythApi.Gods.DBRepositories;
using MythApi.Common.Database;
using MythApi.Endpoints.v1;
using MythApi.Mythologies.DBRepositories;
using MythApi.Mythologies.Interfaces;
using Azure.Identity;
using Serilog;
using System.Runtime.CompilerServices;
using Microsoft.Data.Sqlite;

[assembly: InternalsVisibleTo("IntegrationTests")]

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MythApi")
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
    .MinimumLevel.Warning()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting up the application!");

    // Parse command line arguments
    var inMemoryDatabase = args.Contains("--in-memory-database");
    var sqliteDatabase = true; // Default to demo
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    

    // Configure database context based on the flags
    if (inMemoryDatabase)
    {
        // Configure in-memory database
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseInMemoryDatabase("MythApiInMemoryDb");
        });

        // Add a singleton service for database seeding
        builder.Services.AddSingleton<DatabaseInitializer>();

        Log.Information("Using in-memory database");
    }
    else if (sqliteDatabase || builder.Environment.IsDevelopment())
    {
        // Configure SQLite database
        var sqlitePath = Path.Combine(AppContext.BaseDirectory, "mythapi.db");
        var connectionString = $"Data Source={sqlitePath}";

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlite(connectionString);
        });

        // Add a singleton service for database seeding
        builder.Services.AddSingleton<DatabaseInitializer>();

        Log.Information($"Using SQLite database at: {sqlitePath}");
    }
    else
    {
        // Configure PostgreSQL database (existing code)
        var keyVaultName = builder.Configuration["MYTH_KeyVaultName"];
        var uri = new Uri($"https://{keyVaultName}.vault.azure.net/");
        var credential = new DefaultAzureCredential();

        builder.Configuration.AddAzureKeyVault(uri, credential);

        var connectionString = $"Host={builder.Configuration["dbHost"]};Database={builder.Configuration["dbName"]};Username={builder.Configuration["adminUsername"]};Password={builder.Configuration["adminPassword"]}";

        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        Log.Information("Using PostgreSQL database");
    }

    builder.Services
        .AddScoped<IGodRepository, GodRepository>()
        .AddScoped<IMythologyRepository, MythologyRepository>();

    var app = builder.Build();

    // Create/migrate database and initialize with default mythologies if needed
    if (inMemoryDatabase || sqliteDatabase || builder.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // For SQLite, we need to ensure the database is created
        if (sqliteDatabase || builder.Environment.IsDevelopment())
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
            Log.Information("SQLite database schema created");
        }

        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        initializer.InitializeDatabase();
    }

    app.RegisterGodEndpoints();
    app.RegisterMythologiesEndpoints();
    app.UseSwagger();
    app.UseSwaggerUI();

    

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.CloseAndFlush();
}
