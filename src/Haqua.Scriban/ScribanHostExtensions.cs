using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Haqua.Scriban;

public static class ScribanHostExtensions
{
    /// <summary>
    /// Load scriban template.
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    /// <returns>The same instance of the <see cref="IHost" /> for chaining.</returns>
    public static async Task<IHost> UseScribanTemplateAsync(this IHost host)
    {
        var scribanService = host.Services.GetService<ScribanTemplate>();
        if (scribanService == null)
        {
            throw new NullReferenceException(nameof(ScribanTemplate));
        }

        await scribanService.LoadTemplateFromDirectoryAsync().ConfigureAwait(false);
        return host;
    }
}