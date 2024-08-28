var path = "chirp_cli_db.csv";

//https://learn.microsoft.com/en-us/dotnet/api/system.io.streamreader?view=net-8.0
if (args[0] == "read")
{
    try
    {
        // Open the text file using a stream reader.
        using StreamReader reader = new(path);

        // Read the stream as a string.
        reader.ReadLine();
        string line;
        // Read and display lines from the file until the end of
        // the file is reached.
        while ((line = reader.ReadLine()) != null)
        {
            Console.WriteLine(line);
        }
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
}

//https://learn.microsoft.com/en-us/dotnet/api/system.io.file.appendtext?view=net-7.0
if (args[0] == "cheep")
{
    using (StreamWriter sw = File.AppendText(path))
    {
        DateTimeOffset utcTime = DateTimeOffset.UtcNow;
        sw.WriteLine(Environment.UserName + "," + args[1] + "," + utcTime.ToUnixTimeSeconds());
    }	
}