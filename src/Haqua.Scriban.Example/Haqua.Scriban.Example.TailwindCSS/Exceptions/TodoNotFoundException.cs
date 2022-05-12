namespace Haqua.Scriban.Example.TailwindCSS.Exceptions;

public class TodoNotFoundException : Exception
{
    public TodoNotFoundException(Guid id) : base($"Todo {id} not found.")
    {
    }
}