using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using CsvHelper;
using SimpleDB;

public class Program
{
    static String path = "chirp_cli_db.csv";
	
    static async Task<int> Main(string[] args)
    {
        // We are using System.CommandLine, info can be found here:
        // https://learn.microsoft.com/en-us/dotnet/standard/commandline/get-started-tutorial
        // used command "dotnet add package System.CommandLine --version 2.0.0-beta4.22272.1" to add System.CommandLine to project
        
        // Read Command
        var readCommand = new Command("read", "Reads Chirps from the database."); 
        readCommand.SetHandler( () => Read.run());
        
        // Chirp command
        var chirpOption = new Option<string>("--m", "The given string to chirp.");
        var chirpCommand = new Command("chirp", "Cheeps the given arguments.") { chirpOption };
        chirpCommand.SetHandler( (file) => Cheep.run(file!),chirpOption);
        
        // Root command
        var rootCommand = new RootCommand("Chirp CLI Project");
        rootCommand.AddCommand(readCommand);
        rootCommand.AddCommand(chirpCommand);

        return await rootCommand.InvokeAsync(args);
    }


    public record Cheepe(string Author, string Message, string Timestamp);
    class Read
    {
        public static void run()
		{ 
			IDatabaseRepository<Cheepe> db = new CSVDatabase<Cheepe>();
			var records = db.Read();

			foreach(var record in records)
			{
				var author = record.Author;
				var timestamp = getDateFormatted(record.Timestamp);
				var message = record.Message;

                Console.WriteLine(author + " @ " + timestamp + ": " + message);
			}
        }

        public static String getDateFormatted(String inpt)
		{
            String format = "dd'/'MM'/'yyyy HH:mm:ss"; // https://www.c-sharpcorner.com/blogs/date-and-time-format-in-c-sharp-programming1
            String date = "date error";
            try {
                date = DateTimeOffset.FromUnixTimeSeconds(int.Parse(inpt)).ToLocalTime().ToString(format);
            } catch(Exception e) {
                Console.Error.WriteLine(e);
            }
            return date;
        }

        public static String info() {
            return "'read' - Reads and prints current chirps.";
        }
        
    } 
    
    class Cheep {
        public static void run(String args)
		{
          	IDatabaseRepository<Cheepe> db = new CSVDatabase<Cheepe>();

			var author = Environment.UserName;
            var message = args;
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var cheep = new Cheepe(author, message, timestamp);

			db.Store(cheep);
        }
        
        public static String info()
        {
            return "'cheep' - Cheeps your second argument.";
        }
      
    }

    class Help {
        public static void run() {
            Console.WriteLine("List of available commands:");
            Console.WriteLine(Read.info());
            Console.WriteLine(Cheep.info());
            Console.WriteLine(Help.info());
        }

        public static String info()
        {
            return "'help' - Lists available commands.";
        }
    }
    
    
}