using Scriban;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Haqua.Scriban;

internal class IncludeFromDictionary : ITemplateLoader
{
    public string GetPath(TemplateContext context, SourceSpan callerSpan, string templateName)
    {
        return templateName;
    }

    public string Load(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        return "";
    }

    public ValueTask<string> LoadAsync(TemplateContext context, SourceSpan callerSpan, string templatePath)
    {
        return ValueTask.FromResult("");
    }
}