using System.Globalization;
using System.Text.RegularExpressions;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

public class Program { //
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

    class Read
    {
        public static void run() { 
            // regex taken from https://stackoverflow.com/questions/3507498/reading-csv-files-using-c-sharp/34265869#34265869
            // added |\n for newlines TODO: Later this will not work, people will want to chirp with linebreaks.
            try {
                using (StreamReader reader = File.OpenText(path)) {
                    
                    Regex regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))|\n");

                    String[] csvData = regex.Split(reader.ReadToEnd());

                    csvData = csvData.Take(csvData.Length-1).Skip(3).ToArray(); // remove 3 first elements and last whitespace
                    
                    for(int i = 0; i < csvData.Length; i+=3) {
                        String date = getDateFormatted(csvData[i+2]); 
                        Console.WriteLine(csvData[i]+" @ "+date+": "+csvData[i+1]);
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
                date = DateTimeOffset.FromUnixTimeSeconds(int.Parse(inpt)).ToLocalTime().toString(format);
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
        public static void run(String args) //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendtext?view=net-7.0
        {
            using (StreamWriter sw = File.AppendText(path)) {
                DateTimeOffset utcTime = DateTimeOffset.UtcNow;

                String message = args; 
                
                sw.WriteLine(Environment.UserName + ",\"" + message + "\"," + utcTime.ToUnixTimeSeconds());
                Console.WriteLine("Successfully cheeped \""+message+"\".");
            }
        }
        
        public static String info()
        {
            return "'cheep' - Cheeps your second argument.";
        }
      
    }
}