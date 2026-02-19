# Steps taken


## Creating project 

```powershell
dotnet new web -o MythApi -f net8.0
```

## Importing packages 

```
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```

```powershell
# To build image
docker build -f Dockerfile . -t mythapi-image

# To create container
docker create --name mythapi-core mythapi-image

# To run the container with the image
docker run -d -p 5001:80 --name mythapi-container mythapi-test
```

```powershell
docker init
# <Make changes to compose.yaml>
docker compose up --build
```

```powershell
# install dotnet ef
dotnet tool install --global dotnet-ef

# create first migration
# pre requisite:
# dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0

# Create first migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## Setting up Azure Key Vault

Installing dependencies: 
```powershell
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
# dotnet add package Azure.Security.KeyVault.Secrets
dotnet add package Azure.Identity
```

Setting the environment variable 
```powershell
$Env:KEY_VAULT_NAME="<your-key-vault-name>"
```


## Setting up access to key vault

Used DefaultAzureCredentials to connect to key vault to retrieve secrets. 

For local running I could just use ```az login``` to get the neccesary credentials (after first giving myself the access "Key Vault Secrets User" role in Azure to the KeyVault).

For running it in a container locally I created a service principal and used it's Client ID and Client Secret (as well as Tenant Id) in environment variables, that way DefualtAzureCredentials picked it up.
```yaml
    environemt:
      - MYTH_KeyVaultName=${MYTH_KeyVaultName}
      - AZURE_CLIENT_ID=${AZURE_CLIENT_ID}
      - AZURE_CLIENT_SECRET=${AZURE_CLIENT_SECRET}
      - AZURE_TENANT_ID=${AZURE_TENANT_ID
}```