using Microsoft.Extensions.DependencyInjection;
namespace DrMadWill.Layers.Extensions;

public static class WebSitePolicy
{
    public static void AddPolicies(this IServiceCollection services,string name,string[] externalApps)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name,
                builder => builder.WithOrigins(externalApps)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });
    }
}