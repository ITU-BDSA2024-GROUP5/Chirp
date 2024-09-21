namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheep(Cheep cheep) {
        Console.WriteLine(formatCheep(cheep));
    }
    
    public static string GetDateFormatted(string unixTimeSeconds)
    {
        // https://www.c-sharpcorner.com/blogs/date-and-time-format-in-c-sharp-programming1
        string format = "dd'/'MM'/'yyyy HH:mm:ss"; 
        string date = "date error";
        
        try {
            date = DateTimeOffset.FromUnixTimeSeconds(int.Parse(unixTimeSeconds)).ToLocalTime().ToString(format);
        } catch(Exception e) {
            Console.Error.WriteLine(e);
        }
        
        return date;
    }

    public static String formatCheep(Cheep cheep) {
        return cheep.Author + " @ " + GetDateFormatted(cheep.Timestamp) + ": " + cheep.Message;
    }
    
}