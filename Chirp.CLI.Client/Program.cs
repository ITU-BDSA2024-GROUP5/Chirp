using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using CsvHelper;

public class Program { //
    static String path = "chirp_cli_db.csv";
    
    static async Task<int> Main(string[] args)
    
    public static void Main(string[] args)
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


    public record Cheepe(string Author, string Message, String Timestamp);
    class Read
    {
        public static void run() { 
            // https://joshclose.github.io/CsvHelper/getting-started/
            try {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Cheepe>();
                
                    foreach (var record in records)
                    {
						var author = record.Author;
						var timestamp = getDateFormatted(record.Timestamp);
						var message = record.Message;
                        Console.WriteLine(author + " @ " + timestamp + ": " + message);
                    }
                }
            } catch (IOException e) {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public static String getDateFormatted(String inpt) {
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
        public static void run(String[] args) //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendtext?view=net-7.0
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Not enough arguments provided to cheep.");
                return; 
            }
            
            using (var stream = File.Open(path, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            {
                var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = stream.Length == 0
                };
                var author = Environment.UserName;
                var message = args[1];
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                var cheep = new Cheepe(author, message, timestamp);
                var records = new List<Cheepe>();
                records.Add(cheep);
                
                using (var csv = new CsvWriter(writer, config))              
                {                                                            
                    csv.WriteRecords(records);                               
                }               
                Console.WriteLine("Successfully cheeped \"" + args[1] + "\".");
            }
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