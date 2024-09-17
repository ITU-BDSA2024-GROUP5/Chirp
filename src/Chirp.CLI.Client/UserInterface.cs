namespace Chirp.CLI;

public static class UserInterface
{
    public static void PrintCheep(Program.Cheep cheep) {
        var author = cheep.Author;
        var timestamp = GetDateFormatted(cheep.Timestamp);
        var message = cheep.Message;

        Console.WriteLine(author + " @ " + timestamp + ": " + message);
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
    
}