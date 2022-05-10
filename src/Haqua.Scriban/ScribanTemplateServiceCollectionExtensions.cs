using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Haqua.Scriban;

public static class ScribanTemplateServiceCollectionExtensions
{
    /// <summary>
    /// Adds scriban template service to the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <returns>The same instance of the <see cref="IServiceCollection" /> for chaining.</returns>
    public static IServiceCollection AddScribanTemplate(
        this IServiceCollection services,
        ScribanTemplateOptions? options = null)
    {
        services.TryAddSingleton(provider =>
        {
            if (options?.FileProvider != null)
            {
                return new ScribanTemplate(options);
            }

            var webHostEnv = provider.GetService<IWebHostEnvironment>();
            var fileProvider = webHostEnv!.ContentRootFileProvider;

            if (options == null)
            {
                return new ScribanTemplate(new ScribanTemplateOptions { FileProvider = fileProvider });
            }

            options.FileProvider = fileProvider;
            return new ScribanTemplate(options);
        });

        return services;
    }
}