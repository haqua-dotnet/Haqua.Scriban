using Haqua.Scriban;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScribanTemplate();

var app = builder.Build();

await app.UseScribanTemplateAsync();

app.UseStaticFiles();

app.MapGet("/", () => Results.Extensions.ScribanView("pages/home.html", new { Name = "Scriban Template" }));

app.Run();