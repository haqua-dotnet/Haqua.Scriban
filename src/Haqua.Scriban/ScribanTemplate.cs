using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Scriban;
using Scriban.Runtime;
using WebMarkupMin.Core;

namespace Haqua.Scriban;

public class ScribanTemplate
{
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

    public async Task<string> RenderAsync(string viewPath, object? model = null)
    {
        if (_templates.Count == 0)
        {
            await LoadTemplateFromDirectory().ConfigureAwait(false);
        }

        var scriptObject = new ScriptObject { ["model"] = model };

        var context = new TemplateContext { TemplateLoader = _templateLoader };
        context.PushGlobal(scriptObject);

        foreach (var template in _templates)
        {
            context.CachedTemplates.Add(template.Key, template.Value);
        }

        var result = await _templates[viewPath].RenderAsync(context).ConfigureAwait(false);
        return result;
    }

    private async Task LoadTemplateFromDirectory()
    {
        if (!_options.FileProvider!.GetDirectoryContents("views").Exists)
        {
            throw new DirectoryNotFoundException(_options.FileProvider.GetFileInfo("views").PhysicalPath);
        }

        _templates.Clear();

        foreach (var file in _options.FileProvider!.GetRecursiveFiles("views"))
        {
            await LoadTemplate(file).ConfigureAwait(false);
        }
    }

    private async Task LoadTemplate(IFileInfo file)
    {
        await using var readStream = file.CreateReadStream();
        using var streamReader = new StreamReader(readStream);

        var fileValue = await streamReader.ReadToEndAsync().ConfigureAwait(false);

        var fileName = file.PhysicalPath
            .Replace(_options.FileProvider!.GetFileInfo("views").PhysicalPath, "")
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
            () => LoadTemplateFromDirectory().ConfigureAwait(false));
    }
}