using FormSubmissions.Domain.Interfaces;
using FormSubmissions.Infrastructure.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FormSubmissions.API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddFormSubmissions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);
        services.AddScoped<IFormDefinitionRepository, FormSubmissions.Infrastructure.Persistence.Repositories.FormDefinitionRepository>();
        services.AddScoped<ISubmissionRepository, FormSubmissions.Infrastructure.Persistence.Repositories.SubmissionRepository>();
        services.AddScoped<FormSubmissions.Domain.Services.SubmissionDomainService>();
        return services;
    }
}
