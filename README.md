
# MythAPI

## Motivation

This repository was created to refresh on my .NET knowledge, better understand Docker and containerized apps, as well as GitHub Actions, and become more familiar with Microsoft's Entity Framework. The domain of the API was chosen more or less at random, as it wasn't deemed important for this project, and I wanted to get away from the old shop example.

# Walkthrough

# Creating the project

To create a base app to start of the project I used the command `dotnet new web -o MythApi -f net8.0`. Here I specify that I want to create a new project, following the `web` template and that it should use the framework `.NET 8.0`. Also specified is the name of the project, which in this case is `MythApi`. This gives us a brand new .NET api, which already follow the minimal API approach.

After creating the project, we should enter the project root folder in our terminal window. In this case with `cd MythApi`. 

To run this project you can run the following commands:

```powershell
# Optional
dotnet build

# To run
dotnet run
```
To stop the running you can press <kbd>Ctrl</kbd> + <kbd>C</kbd>

# Importing the neccesary packages 

As with any project, one needs to use the correct packages to easliy code the application and to use some great features.

## Swashbuckle

I installed Swashbuckle to use swagger. That makes it easier to navigate the available endpoints, and to perform simple tests of the API.

```powershell
dotnet add package Swashbuckle.AspNetCore 

# Alternatively one can also specify the package version
dotnet add package Swashbuckle.AspNetCore --version 6.5.0 
```

## Entity Framework

In this project we have been using Entity Framework for communication with the database, model binding and also for creating the database tables. The specific package that one should use depends on the database technology one is using. In this project we are using Postgresql and are therefore using the Entity Framework package specifically for PostgreSQL.

```
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0
```

## Authentication

We are using the Azure Identity functionality to connect to Azure resources, that we will use in this application. We will also be using a Key Vault to store configuration variables, so that we don't need to confiugure an AppSetting file etc.

```powershell
# For using the Azure Identity
dotnet add package Azure.Identity

# For using Key Vault secrets in the App Configuration
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```


# Setting up Docker

To be able to containerize this API we want to use Docker. To set it up we will run the command `docker init` (Requires docker to be installed). This will provide a step by step menu that will provide one with the neccesary files for your application. 

You'll most likely end up with the following files:

```
- .dockerignore
- Dockerfile
- compose.yaml
- README.Docker.md
```

To run the application using docker you can run the following command: 

```
docker compose up --build
```

To stop the running you can press <kbd>Ctrl</kbd> + <kbd>C</kbd>

The argument `--build` forces docker to rebuild the application, and not simply reuse the last built image. To tear down the images you have created you can run the command 

```
docker compose down
```

# Structure

In this project I have experimented with structuring code around domains instead of around the service layers, which is usual.

## Setting up swagger

To set up swagger is quite easy. We open the `Program.cs` file and add the lines 

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```

and after the app is build we add these lines

```csharp
app.UseSwagger();
app.UseSwaggerUI();
```

Running the application and then, using a browser, navigating to `http://localhost:<port>/swagger` should display the swagger page.

## Setting up Configuration

So to be able to access the Key Vault to fetch the secrets as for the application configuration you should add the following lines to the `Program.cs` file.

```csharp
var keyVaultName = builder.Configuration["MYTH_KeyVaultName"];
var uri = new Uri($"https://{keyVaultName}.vault.azure.net/");
var credential = new DefaultAzureCredential();

builder.Configuration.AddAzureKeyVault(uri, credential);
```

Here we fetch the name of the Key Vault from the config. This can be given either in the `appsettings.json` file, or as we have done, through environment variables. To specify the environment variable you can run `$ENV:MYTH_KeyVaultName="<the name of your key vault>"`.

We are using the DefaultAzureCredentials to get access to the Key Vault. This is the most flexible way of authenticating as we can use the credentials on your machine (if you have loged on using az login), managed identity (if the code runs in an Azure resource) or from environment variables.

It is important that the credentials that are given has access to the Key Vault, and can read the secrets. Ensure that the "user" has the access `Key Vault Secrets User` for the specific Azure Key Vault.

To run this locally in docker we have specified environment variables to be used by the credentials.

First we specify the environment variables in our terminal window
```powershell
$Env:AZURE_TENANT_ID="<The tenant ID of your tenant>"
$Env:AZURE_CLIENT_ID="<The client id of the service principal you wish to be using>"
$Env:AZURE_CLIENT_SECRET="<The client secret of the serivce principal>"
```

In the `compose.yaml` file we define the environment variables so that it's accessible also from the docker image.

We add the following lines to the compose file
```yaml
    environemt:
      - MYTH_KeyVaultName=${MYTH_KeyVaultName}
      - AZURE_CLIENT_ID=${AZURE_CLIENT_ID}
      - AZURE_CLIENT_SECRET=${AZURE_CLIENT_SECRET}
      - AZURE_TENANT_ID=${AZURE_TENANT_ID
```

After this the compose file should look something like this

```yaml
services:
  server:
    build:
      context: .
    ports:
      - 8080:8080
    environment:
      - MYTH_KeyVaultName=${MYTH_KeyVaultName}
      - AZURE_CLIENT_ID=${AZURE_CLIENT_ID}
      - AZURE_CLIENT_SECRET=${AZURE_CLIENT_SECRET}
      - AZURE_TENANT_ID=${AZURE_TENANT_ID}
```

Run the application and verify that you don't get any errors when running.

## Create the Database Layer

You need to already have a postgres database available for this step. 

We will now set up the database layer, using Entity Framework. 

We configure new folder and files for this code. 
```
.\
|
|-- \Common
  |-- \Database
    |-- AppDBContext.cs
    |-- \Models
      |-- God.cs
      |-- Mythology.cs
```

The AppDBContext will be the main file for mapping to the database we create, and will be used by the application to read and write data to and from the database.

In the Models folder we specify all the Database table models that we'll be using.

### Create the models

We specify the models, how we wish them to apear in the database

#### Mythology
```csharp
// Defines namespace
namespace MythApi.Common.Database.Models;

public class Mythology {
    // ID of each object (in database)
    public int Id { get; set; }
    // Name
    public string Name { get; set; } = null!;
    // List of all gods (we have a one-to-many relation)
    public List<God> Gods { get; set; } = [];
}
```

#### God

```csharp

namespace MythApi.Common.Database.Models;

public class God {
    
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    // Reference to the Mythology table
    public int MythologyId { get; set; }
}
```

### Create the DbContext

We first create a new class that extends the Entity Framework class `DbContext`.

```csharp
using Microsoft.EntityFrameworkCore;

namespace MythApi.Common.Database;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

}
```

Next we add the models as tables using the `DbSet` by adding the following to the class.

```csharp
public DbSet<God> Gods { get; set; } = null!;
public DbSet<Mythology> Mythologies { get; set; } = null!;
```

Next we override the `OnModelCreating` method to configure the data mapping.

We add the following code to our `AppDBContext` class:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder) {
    // Map entities to tables
    modelBuilder.Entity<Mythology>().ToTable("Mythology");
    modelBuilder.Entity<God>().ToTable("God");
    
    // Map relation between the tables
    modelBuilder.Entity<Mythology>()
        .HasMany(e => e.Gods)
        .WithOne()
        .HasForeignKey(e => e.MythologyId)
        .IsRequired()
        ;

    // Passes this on to the base class
    base.OnModelCreating(modelBuilder);
}
```

The code should look something like this now:

```csharp
using Microsoft.EntityFrameworkCore;
using MythApi.Common.Database.Models;

namespace MythApi.Common.Database;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<God> Gods { get; set; } = null!;
    public DbSet<Mythology> Mythologies { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Mythology>().ToTable("Mythology");
        modelBuilder.Entity<God>().ToTable("God");
        
        modelBuilder.Entity<Mythology>()
            .HasMany(e => e.Gods)
            .WithOne()
            .HasForeignKey(e => e.MythologyId)
            .IsRequired()
            ;

        base.OnModelCreating(modelBuilder);
    }
}
```

### Adding the Database context to the App

We now go to the `Program.cs` file to add the context to the application. 

We add these lines 
```csharp
var connectionString = $"Host={builder.Configuration["dbHost"]};Database={builder.Configuration["dbName"]};Username={builder.Configuration["adminUsername"]};Password={builder.Configuration["adminPassword"]}";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
```

This use the config values `dbHost`, `dbName`, `adminUsername`, and `adminPassword`, which we in this case fetch from the Key Vault we have configured to use earlier.

### Create a Migration

Now that the database is configured we should be able to run a EntityFramework migration. 
The migration can create and update the tables in the database.

To migrate you need to have the `dotnet-ef` extension installed. To do that run

```powershell
dotnet tool install --global dotnet-ef
```

Once that is installed you can run 
```powershell
# Create a migration
dotnet ef migrations add <Migration name>
# Update the database with the latest migration
dotnet ef database update
```

To start of we'll name the first migration "InitialCreate" the commands would then be 
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```


## Create Infrastructure

We'll create this domain specific structure, so at root level we specify a folder for each domain, and create all data for each domain within this. 

### Mythologies

#### Interface

We create a folder called `Interfaces` in the `Mythologies` folder. Here we create a file called `IMythologyRepository.cs` which should be the interface for the mythology repository. We define this with a method returning all Mythologies in the database. We'll just return the Mythology object from the database models, however we could've created our own object to map to here.

```csharp

using MythApi.Common.Database.Models;

namespace MythApi.Mythologies.Interfaces;

public interface IMythologyRepository
{
    public Task<IList<Mythology>> GetAllMythologiesAsync();
}

```

#### Repository

After creating the interface for the repository class we create an instance of this interface. We do this in a new folder which we call `DBRepository`. 

```csharp
using Microsoft.EntityFrameworkCore;
using MythApi.Common.Database;
using MythApi.Common.Database.Models;
using MythApi.Mythologies.Interfaces;

namespace MythApi.Mythologies.DBRepositories;

public class MythologyRepository : IMythologyRepository
{
    private readonly AppDbContext _context;

    public MythologyRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IList<Mythology>> GetAllMythologiesAsync()
    {
        return await _context.Mythologies.ToListAsync();
    }
}
```

We use the database context we have previously written and we fetch all the mythologies from the context and return these to the caller.

#### Inject

In the `Program.cs` file we inject the repository we've created for the interface we created.

We do this by adding this code.

```csharp
builder.Services.AddScoped<IMythologyRepository, MythologyRepository>();
```

### Gods

TODO : Fill out

## Create Endpoints

We create a seperate folder for endpoints at root-level called `Endpoints` with a sub-folder called `v1` for the first version of the endpoints. In this folder we create a file for each domain we create endpoints for. Here starting with `Mythologies`.

```csharp
using MythApi.Common.Database.Models;
using MythApi.Mythologies.Interfaces;

// Create a class
public static class Mythologies {
    // Create an extension method for registering the endpoints to a route builder. 
    public static void RegisterMythologiesEndpoints(this IEndpointRouteBuilder endpoints) {
        // Defines the group
        var mythologies = endpoints.MapGroup("/api/v1/mythologies");

        // Add a get mapping to get all mythologies. Calls a method we define under
        mythologies.MapGet("", GetAllMythologies); 
    }

    // The method takes an instance that follows the mythology repository interface
    // We then call the 'GetAllMythologiesAsync' method and returns the data
    public static Task<IList<Mythology>> GetAllMythologies(IMythologyRepository repository) => repository.GetAllMythologiesAsync();
} 

```

Back in the `Program.cs` file we call the extension method we just created by adding.
```csharp
app.RegisterMythologiesEndpoints();
```

Now we can run the application and verify that we are able to retrieve data from the endpoint. If there is no entries in the `Mythology` table the result should be an empty list `[]`. One can add some mythologies manually to the table to get some content one can verify.

At the end of this the `Program.cs` file look like this

```csharp
using Microsoft.EntityFrameworkCore;
using MythApi.Gods.Interfaces;
using MythApi.Gods.DBRepositories;
using MythApi.Common.Database;
using MythApi.Endpoints.v1;
using MythApi.Mythologies.DBRepositories;
using MythApi.Mythologies.Interfaces;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var keyVaultName = builder.Configuration["MYTH_KeyVaultName"];
var uri = new Uri($"https://{keyVaultName}.vault.azure.net/");
var credential = new DefaultAzureCredential();

builder.Configuration.AddAzureKeyVault(uri, credential);

var connectionString = $"Host={builder.Configuration["dbHost"]};Database={builder.Configuration["dbName"]};Username={builder.Configuration["adminUsername"]};Password={builder.Configuration["adminPassword"]}";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services
    .AddScoped<IGodRepository, GodRepository>()
    .AddScoped<IMythologyRepository, MythologyRepository>();

var app = builder.Build();

app.RegisterGodEndpoints();
app.RegisterMythologiesEndpoints();
app.UseSwagger();
app.UseSwaggerUI();


app.Run();
```

# Future work

- Improve Bicep scripts
- Set up with kubernetes/image repository etc
- Add API Authentication & Authorization
