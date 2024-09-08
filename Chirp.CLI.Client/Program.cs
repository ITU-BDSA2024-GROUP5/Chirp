﻿namespace Chirp.CLI;

using System.Globalization;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using Chirp.CLI;
using SimpleDB;

public class Program
{
    static async Task<int> Main(string[] args)
    {
        // We are using System.CommandLine, info can be found here:
        // https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial

        // Read Command
        var readOption = new Option<int>("-lim", "Limits the number of cheeps to read.");
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


    public record Cheepe(string Author, string Message, string Timestamp);

    private static void ReadCheeps(int count)
    {
        Console.WriteLine("Reading "+count+" cheeps.");
        
        IDatabaseRepository<Cheepe> db = new CSVDatabase<Cheepe>();
        var records = db.Read();

        foreach (var record in records)
            UserInterface.PrintCheep(record);
    }
    
    private static void ReadCheeps() { ReadCheeps(0); }

    private static void DoCheep(string args)
    {
        IDatabaseRepository<Cheepe> db = new CSVDatabase<Cheepe>();

        var author = Environment.UserName;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        var cheep = new Cheepe(author, args, timestamp);

        db.Store(cheep);
    }
    
}