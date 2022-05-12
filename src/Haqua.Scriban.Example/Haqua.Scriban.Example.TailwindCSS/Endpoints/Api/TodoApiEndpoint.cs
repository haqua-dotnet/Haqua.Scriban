using Haqua.Scriban.Example.TailwindCSS.Dtos;
using Haqua.Scriban.Example.TailwindCSS.Services;

namespace Haqua.Scriban.Example.TailwindCSS.Endpoints.Api;

public static class TodoApiEndpoint
{
    public static IEndpointRouteBuilder MapTodoApi(this IEndpointRouteBuilder route)
    {
        route.MapPost("/api/todo", Add);
        route.MapPost("/api/todo/{id}", Check);
        route.MapDelete("/api/todo/{id}", Delete);

        return route;
    }

    private static IResult Add(InTodoAddDto dto, TodoService todoService)
    {
        if (dto.Todo == null)
        {
            return Results.BadRequest("Todo is required");
        }

        var todo = todoService.Add(dto.Todo);
        return Results.Ok(todo);
    }

    private static IResult Check(Guid id, TodoService todoService)
    {
        var (todo, error) = todoService.Check(id);
        return error != null ? Results.NotFound(error.Message) : Results.Ok(todo);
    }

    private static IResult Delete(Guid id, TodoService todoService)
    {
        var error = todoService.Delete(id);
        return error != null ? Results.NotFound(error.Message) : Results.NoContent();
    }
}