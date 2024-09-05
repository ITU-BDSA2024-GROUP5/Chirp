﻿using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;

public class Program { //
    static String path = "chirp_cli_db.csv";
    
    public static void Main(string[] args)
    {
        if (args.Length <= 0)
        {
            // test for no argument
            Console.WriteLine("No arguments provided, try 'help' for a list of commands.");
            return;
        }


        switch (args[0]) // check first argument (command)
        {
            case "read":
                Read.run();
                break;
            case "help":
                Help.run();
                break;
            case "cheep":
                Cheep.run(args);
                break;
            default:
                Console.WriteLine(args[0] + " not recognized as a command, try 'help' for a list of commands.");
                break;
        }
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