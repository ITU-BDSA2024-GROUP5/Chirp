namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheep(Program.Cheepe cheepe) {
        var author = cheepe.Author;
        var timestamp = GetDateFormatted(cheepe.Timestamp);
        var message = cheepe.Message;

        Console.WriteLine(author + " @ " + timestamp + ": " + message);
    }
    
    private static string GetDateFormatted(string unixTimeSeconds)
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
    
}