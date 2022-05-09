using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haqua.Scriban;

public static class ScribanTemplateServiceCollectionExtensions
{
    public static IServiceCollection AddScribanTemplate(
        this IServiceCollection services,
        ScribanTemplateOptions? options = null)
    {
        services.TryAddSingleton(provider =>
        {
            if (options?.FileProvider == null)
            {
                var webHostEnv = provider.GetService<IWebHostEnvironment>();
                var fileProvider = webHostEnv!.ContentRootFileProvider;

                if (options == null)
                {
                    return new ScribanTemplate(new ScribanTemplateOptions { FileProvider = fileProvider });
                }

                options.FileProvider = fileProvider;
            }

            return new ScribanTemplate(options);
        });

        return services;
    }
}