

using Microsoft.Extensions.DependencyInjection;

namespace MinimalAPI.Tests.Helpers;

public class TestWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // Use an in-memory database for testing
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<StudentContext>();

            // Clear the database before each test
            context.Database.EnsureDeleted();

            // Ensure the database is created before each test
            context.Database.EnsureCreated();
        }

        return host;
    }
}
