using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Scriban;
using Scriban.Runtime;
using WebMarkupMin.Core;

namespace Haqua.Scriban;

public class ScribanTemplate
{
    private const string Views = "views";

    private readonly HtmlMinifier _htmlMinifier;
    private readonly ScribanTemplateOptions _options;
    private readonly IncludeFromDictionary _templateLoader = new();
    private readonly Dictionary<string, Template> _templates = new();

    public ScribanTemplate(ScribanTemplateOptions options)
    {
        _options = options;

        _htmlMinifier = new HtmlMinifier();

        if (_options.WatchChanged)
        {
            WatchViewDirectoryChange();
        }
    }

    /// <summary>
    /// Render template from specified viewPath.
    /// </summary>
    /// <returns>Rendered template</returns>
    public ValueTask<string> RenderAsync(string viewPath, object? model = null)
    {
        var scriptObject = new ScriptObject { ["model"] = model };

        var context = new TemplateContext { TemplateLoader = _templateLoader };
        context.PushGlobal(scriptObject);

        foreach (var template in _templates)
        {
            context.CachedTemplates.Add(template.Key, template.Value);
        }

        return _templates[viewPath].RenderAsync(context);
    }

    internal async Task LoadTemplateFromDirectoryAsync()
    {
        if (!_options.FileProvider!.GetDirectoryContents(Views).Exists)
        {
            throw new DirectoryNotFoundException(_options.FileProvider.GetFileInfo(Views).PhysicalPath);
        }

        _templates.Clear();

        foreach (var file in _options.FileProvider!.GetRecursiveFiles(Views))
        {
            await LoadTemplateAsync(file).ConfigureAwait(false);
        }
    }

    private async Task LoadTemplateAsync(IFileInfo file)
    {
        await using var readStream = file.CreateReadStream();
        using var streamReader = new StreamReader(readStream);

        var fileValue = await streamReader.ReadToEndAsync().ConfigureAwait(false);

        var fileName = file.PhysicalPath
            .Replace(_options.FileProvider!.GetFileInfo(Views).PhysicalPath, "")
            .TrimStart(Path.DirectorySeparatorChar)
            .Replace("\\", "/");

        if (_options.MinifyTemplate)
        {
            var viewMinified = _htmlMinifier.Minify(fileValue, false);
            _templates[fileName] = Template.Parse(viewMinified.MinifiedContent);
        }
        else
        {
            _templates[fileName] = Template.Parse(fileValue);
        }
    }

    private void WatchViewDirectoryChange()
    {
        ChangeToken.OnChange(
            () => _options.FileProvider!.Watch("**/*.html"),
            () => LoadTemplateFromDirectoryAsync().ConfigureAwait(false));
    }
}