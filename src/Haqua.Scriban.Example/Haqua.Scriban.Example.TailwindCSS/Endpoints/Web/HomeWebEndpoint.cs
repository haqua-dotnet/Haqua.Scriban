using Haqua.Scriban.Example.TailwindCSS.Services;

namespace Haqua.Scriban.Example.TailwindCSS.Endpoints.Web;

public static class HomeWebEndpoint
{
    public static IEndpointRouteBuilder MapHomeWeb(this IEndpointRouteBuilder route)
    {
        route.MapGet("/", Get);
        return route;
    }

    private static IResult Get(TodoService todoService)
    {
        return Results.Extensions.ScribanView(
            "pages/index.html",
            new { Todos = todoService.Get() });
    }
}