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
    private readonly Dictionary<string, string> _templates = new();

    private ITemplateLoader? _templateLoader;

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

        var template = Template.Parse(_templates[viewPath]);
        var result = await template.RenderAsync(context).ConfigureAwait(false);

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

        _templateLoader = new IncludeFromDictionary(_templates);
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
            _templates[fileName] = viewMinified.MinifiedContent;
        }
        else
        {
            _templates[fileName] = fileValue;
        }
    }

    private void WatchViewDirectoryChange()
    {
        ChangeToken.OnChange(
            () => _options.FileProvider!.Watch("**/*.html"),
            () => LoadTemplateFromDirectory().ConfigureAwait(false));
    }
}