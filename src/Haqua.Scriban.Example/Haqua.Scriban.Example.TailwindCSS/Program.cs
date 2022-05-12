using Haqua.Scriban;
using Haqua.Scriban.Example.TailwindCSS.Endpoints.Api;
using Haqua.Scriban.Example.TailwindCSS.Endpoints.Web;
using Haqua.Scriban.Example.TailwindCSS.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScribanTemplate();
builder.Services.AddSingleton<TodoService>();

var app = builder.Build();

await app.UseScribanTemplateAsync();

app.UseStaticFiles();

app.MapTodoApi();
app.MapHomeWeb();

app.Run();