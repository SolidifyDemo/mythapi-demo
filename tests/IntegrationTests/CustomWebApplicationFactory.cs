using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        
        // Configure the application to use SQLite for testing
        builder.UseContentRoot(Directory.GetCurrentDirectory());
        
        // Pass the --sqlite-database argument to the application
        // Not working. How pass just a flag and not an argument?
        builder.UseSetting("Args:0", "--sqlite-database");

        builder.ConfigureServices(services =>
        {
            // Add any additional service configuration for testing
        });
    }
}