using FormSubmissions.Infrastructure.Persistence.InMemoryStore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FormSubmissions.Infrastructure.Config;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InMemoryFormStore>();
        services.AddSingleton<InMemorySubmissionStore>();
        services.AddSingleton<FileStorage.IAttachmentStorage, FileStorage.LocalAttachmentStorage>();
        return services;
    }
}
