using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Haqua.Scriban;

public class ScribanView : IResult
{
    private readonly object? _model;
    private readonly int _statusCode;
    private readonly string _views;

    public ScribanView(string views)
    {
        _views = views;
        _statusCode = 200;
    }

    public ScribanView(string views, object model)
    {
        _views = views;
        _model = model;
        _statusCode = 200;
    }

    public ScribanView(string views, object model, int statusCode)
    {
        _views = views;
        _model = model;
        _statusCode = statusCode;
    }

    public ScribanView(string views, int statusCode)
    {
        _views = views;
        _statusCode = statusCode;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        if (httpContext.RequestServices.GetService(typeof(ScribanTemplate)) is not ScribanTemplate scribanTemplate)
        {
            throw new NullReferenceException();
        }

        var template = await scribanTemplate.RenderAsync(_views, _model);

        httpContext.Response.StatusCode = _statusCode;
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(template);

        await httpContext.Response.WriteAsync(template);
    }
}