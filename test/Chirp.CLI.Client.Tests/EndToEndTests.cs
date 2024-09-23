using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Chirp.Cli.Client.Test;

using System.Diagnostics;
using SoloX.CodeQuality.Test.Helpers.Http;
using CsvHelper;
using System.Globalization;
using SimpleDB;
using System.IO;

public class EndToEndTests
{
    public record Cheep(string Author, string Message, string Timestamp);
    
    [Fact] 
    public async Task EndToEnd()
    {
        
        var mockCheep = new Cheep("nickyye", "hej", "11/09/2024 16.44.39");

        // Act
        var mockHttpClient = new HttpClientMockBuilder()
            .WithBaseAddress(new Uri("http://testwebsite"))
            .WithRequest("/cheeps")
            .RespondingJsonContent<List<Cheep>>(
                request => new List<Cheep> { mockCheep }
            )
            .Build();

        var response = await mockHttpClient.GetFromJsonAsync<List<Cheep>>("/cheeps");

        Assert.NotNull(response); 
        Assert.NotEmpty(response);

        string expectedAuthorName = response[0].Author; // Get the first author's name
        
        // Assert
        Assert.Equal("nickyye", expectedAuthorName);
        
    } 
}