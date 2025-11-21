using FormSubmissions.API.Configuration;
using FormSubmissions.API.Controllers;
using FormSubmissions.API.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FormSubmissions.API.Tests;

public sealed class ApiFactory : IAsyncDisposable
{
    private IHost? _host;

    public async Task<HttpClient> CreateClientAsync()
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(cfg =>
            {
                cfg.AddInMemoryCollection(new Dictionary<string, string?>());
            })
            .ConfigureWebHostDefaults(web =>
            {
                web.UseEnvironment("Development");
                web.UseTestServer();

                web.ConfigureServices((ctx, services) =>
                {
                    services
                        .AddControllers()
                        .AddApplicationPart(typeof(FormsController).Assembly);

                    services.AddFormSubmissions(ctx.Configuration);
                });

                web.Configure(app =>
                {
                    app.UseMiddleware<ExceptionHandlingMiddleware>();
                    app.UseRouting();
                    app.UseEndpoints(e => e.MapControllers());
                });
            });

        _host = await builder.StartAsync();
        return _host.GetTestClient();
    }

    public async ValueTask DisposeAsync()
    {
        if (_host != null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }
}
