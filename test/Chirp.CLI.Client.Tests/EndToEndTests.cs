namespace Chirp.Cli.Client.Test;

using System.Diagnostics;
using CsvHelper;
using System.Globalization;

public class EndToEndTests
{
    public record Cheep(string Author, string Message, string Timestamp);
    [Fact]
    public void EndToEnd()
    {
        // Arrange
        ArrangeTestDatabase();
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "bin/Debug/net7.0/Chirp.CLI.dll read 10";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "src/Chirp.CLI.Client/";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string fstCheep = output.Split("\n")[0];
        // Assert
        Assert.StartsWith("hej", fstCheep);
        Assert.EndsWith("hej3", fstCheep);
        
        File.Delete("../../../../../data/TestDatabase.csv");
    }
    
    private void ArrangeTestDatabase()
    {
        var path = "../../../../../data/TestDatabase.csv";
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Cheep>();
            csv.NextRecord();
        }

        var db = new CsvDatabase<Cheep>();
        db.SetPath(path);
        db.store(new Cheep("hej", "hej2", "hej3"));
    }
}