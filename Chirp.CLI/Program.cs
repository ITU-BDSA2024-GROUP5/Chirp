using System.Globalization;
using System.Text.RegularExpressions;

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


    class Read
    {
        public static void run() { 
            // regex taken from https://stackoverflow.com/questions/3507498/reading-csv-files-using-c-sharp/34265869#34265869
            // added |\n for newlines TODO: Later this will not work, people will want to chirp with linebreaks.
            try {
                using (StreamReader reader = File.OpenText(path)) {
                    
                    Regex regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))|\n");

                    String[] csvData = regex.Split(reader.ReadToEnd());

                    csvData = csvData.Skip(3).ToArray(); // remove 3 first elements
                    
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
                date = DateTimeOffset.FromUnixTimeSeconds(int.Parse(inpt)).ToString(format);
            } catch(Exception e) {
                Console.Error.WriteLine(e);
            }
            return date;
        }

        public static String info() {
            return "'read' - Reads and prints current chirps.";
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
    
    class Cheep {
        public static void run(String[] args) //https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendtext?view=net-7.0
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Not enough arguments provided to cheep.");
                return; 
            }
            
            
            using (StreamWriter sw = File.AppendText(path))
            {
                DateTimeOffset utcTime = DateTimeOffset.UtcNow;
                sw.WriteLine(Environment.UserName + "," + args[1] + "," + utcTime.ToUnixTimeSeconds());
                Console.WriteLine("Sucessfully cheeped '"+args[1]+"'");
            }
        }
        
        public static String info()
        {
            return "'cheep' - Cheeps your second argument.";
        }
      
    }
}