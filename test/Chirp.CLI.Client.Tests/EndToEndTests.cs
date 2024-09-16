namespace Chirp.Cli.Client.Test;

using System.Diagnostics;
using CsvHelper;
using System.Globalization;
using SimpleDB;
using System.IO;

public class EndToEndTests
{
    public record Cheep(string Author, string Message, string Timestamp);
    [Fact] 
    public void EndToEnd()
    {
        // Arrange
        // Act
        string output = "";
        using (var process = new Process())
        {
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "./src/Chirp.CLI.Client/bin/Debug/net7.0/Chirp.CLI.dll read";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.WorkingDirectory = "../../../../../";
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            // Synchronously read the standard output of the spawned process.
            StreamReader reader = process.StandardOutput;
            output = reader.ReadToEnd();
            process.WaitForExit();
        }
        string fstCheep = output.Split("\n")[1];
        // Assert
        Assert.StartsWith("nickyye", fstCheep);
        Assert.EndsWith("test with csv append to file", fstCheep);
    }
}