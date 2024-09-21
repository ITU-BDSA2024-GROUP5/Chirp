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
        IEnumerable<Cheep> records;
        
        
        // Stuff
        app.MapGet("/", () => "Mainpage. Nothing to find here.");
        app.MapGet("/help", () => "Use /cheeps to get all cheeps. Use /cheeps?count=5 to get the last 5 cheeps. You can also post a cheep to /cheep.");
        app.MapGet("/cheeps", (int? count) => 
        {
            if (count > 0)
                records = db.Read(count);
            else
                records = db.Read();

            return records;
        });

        app.MapPost("/cheep", (Cheep cheep) =>
        {
            db.Store(cheep);
        });

        app.Run();
    }
    public record Cheep(string Author, string Message, string Timestamp);
}



