using Haqua.Scriban.Example.TailwindCSS.Exceptions;
using Haqua.Scriban.Example.TailwindCSS.Models;

namespace Haqua.Scriban.Example.TailwindCSS.Services;

public class TodoService
{
    private readonly List<TodoModel> _todos = new()
    {
        new TodoModel
        {
            Id = Guid.NewGuid(),
            Todo = "Lorem ipsum",
            IsChecked = false
        }
    };

    public IReadOnlyList<TodoModel> Get()
    {
        return _todos;
    }

    public TodoModel Add(string todoValue)
    {
        var todo = new TodoModel
        {
            Id = Guid.NewGuid(),
            Todo = todoValue,
            IsChecked = false
        };

        _todos.Add(todo);
        return todo;
    }

    public (TodoModel?, Exception?) Check(Guid id)
    {
        var todo = _todos.FirstOrDefault(e => e.Id == id);
        if (todo == null)
        {
            return (null, new TodoNotFoundException(id));
        }

        todo.IsChecked = !todo.IsChecked;
        return (todo, null);
    }

    public Exception? Delete(Guid id)
    {
        var todoIndex = _todos.FindIndex(e => e.Id == id);
        if (todoIndex == -1)
        {
            return new TodoNotFoundException(id);
        }

        _todos.RemoveAt(todoIndex);
        return null;
    }
}