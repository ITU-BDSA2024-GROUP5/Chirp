namespace Chirp.CLI;

using System.Globalization;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Chirp.CLI;
using SimpleDB;

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
public class Program
{
    static async Task<int> Main(string[] args)
    {
        // We are using System.CommandLine, info can be found here:
        // https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial

        // Read Command
        var readOption = new Option<int>("-lim", "Limits the number of cheeps to read."); //mangler stadig at limit? den læser alle cheeps
        var readCommand = new Command("read", "Reads Chirps from the database.") { readOption }; 
        readCommand.SetHandler( (file) => ReadCheeps(file),readOption);
        
        // Chirp command
        var chirpOption = new Option<string>("-m", "The given string to chirp.");
        var chirpCommand = new Command("chirp", "Cheeps the given arguments.") { chirpOption };
        chirpCommand.SetHandler( (file) => DoCheep(file!),chirpOption);
        
        // Root command
        var rootCommand = new RootCommand("Chirp CLI Project");
        rootCommand.AddCommand(readCommand);
        rootCommand.AddCommand(chirpCommand);

        return await rootCommand.InvokeAsync(args);
    }
    

    async private static void ReadCheeps(int count)
    {
        // Create an HTTP client object
        var baseURL = "http://localhost:5249";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);

        // Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
        // JSON object in the body of the response
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("read");
        foreach (var cheep in cheeps)
        {
            UserInterface.PrintCheep(cheep);
        }
    }
    
    private static void ReadCheeps() { ReadCheeps(0); }

    private static void DoCheep(string args)
    {
        IDatabaseRepository<Cheep> db = CSVDatabase<Cheep>.Instance;

        var author = Environment.UserName;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var cheep = new Cheep(author, args, timestamp);

        db.Store(cheep);
    }
    
}

public record Cheep(string Author, string Message, string Timestamp);