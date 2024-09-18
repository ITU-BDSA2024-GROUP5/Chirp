namespace CsvDBService;

using CsvDBService;
using SimpleDB;

public class Program
{

    public static void Main(String[] args)
    {
        // WEB
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();
        
        // DataBase
        IDatabaseRepository<Cheep> db = CSVDatabase<Cheep>.Instance;
        IEnumerable<Cheep> records = db.Read();
        List<Cheep> cheeps = records.ToList();
        
        
        // Stuff
        app.MapGet("/", () => "Mainpage. Nothing to find here.");
        app.MapGet("/read", () => records);
        app.MapPost("/cheep", (string arg) =>
        {
            var author = Environment.UserName;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var cheep = new Cheep(author, arg, timestamp);
            db.Store(cheep);
        }
            );

        app.Run();
    }
    public record Cheep(string Author, string Message, string Timestamp);
    
}



