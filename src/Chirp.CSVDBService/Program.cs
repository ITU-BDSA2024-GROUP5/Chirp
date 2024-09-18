var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/cheeps", () => new Cheep("mæ", "hej", 1684229348));
app.MapPost("/cheep", () => new Cheep("mæ2", "hej", 1684229348));

app.Run();

public record Cheep(string author, string messasge, long timestamp);
