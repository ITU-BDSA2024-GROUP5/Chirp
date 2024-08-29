public class Program {
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
            try
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            
        }

        public static String info()
        {
            return "'read' - Reads and prints current chirps.";
        }
        
    } 

    class Help
    {
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