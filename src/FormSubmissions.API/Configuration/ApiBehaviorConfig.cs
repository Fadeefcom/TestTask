using Microsoft.Extensions.DependencyInjection;

namespace FormSubmissions.API.Configuration;

public static class ApiBehaviorConfig
{
    public static IServiceCollection AddApiBehavior(this IServiceCollection services)
    {
        services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(o =>
        {
            o.SuppressModelStateInvalidFilter = true;
        });
        return services;
    }
}
