namespace SimpleDB.Tests;

using CsvHelper;
using System.Globalization;
using SimpleDB;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit.Sdk;

public class IntegrationTest1
{
    public record Cheep(string Author, string Message, string Timestamp);
    private void ArrangeTestDatabase()
    {
        var path = "../../../../../data/TestDatabase.csv";
        using (var writer = new StreamWriter(path))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Cheep>();
            csv.NextRecord();
        }

    }
    
    [Fact]
    async public void WriteReadDBTest ()
    {
        //arrange
        ArrangeTestDatabase();
        var db = CSVDatabase<Cheep>.Instance;
        db.SetPath("../../../../../data/TestDatabase.csv");
        
        //act
        var baseURL = "http://localhost:5249";
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(baseURL);
        
        var cheep = new Cheep("testuser", "testcheep", "1726835588");
        var cheepResponse = await client.PostAsJsonAsync("cheep", cheep);
        var cheepStatus = cheepResponse.StatusCode;
        
        var readResponse = await client.GetAsync("cheeps");
        var readStatus = readResponse.StatusCode;
        var cheeps = await client.GetFromJsonAsync<List<Cheep>>("cheeps");
            
        //assert
        Assert.Equal(HttpStatusCode.OK, cheepStatus);
        Assert.Equal(HttpStatusCode.OK, readStatus);
        Assert.Equal("testuser", cheeps.Last().Author);
        Assert.Equal("testcheep", cheeps.Last().Message);
        Assert.Equal("1726835588", cheeps.Last().Timestamp);
        

        File.Delete("../../../../../data/TestDatabase.csv");

    }

    [Fact]
    async public void StoreReadDBTest()
    {
        var db = CSVDatabase<Cheep>.Instance;
        db.SetPath("../../../../../data/TestDatabase.csv");
        db.Store(new Cheep("testuser", "testcheep", "1726835588"));
        var cheeps = db.Read();
        File.Delete("../../../../../data/TestDatabase.csv");
        Assert.Equal("testuser", cheeps.Last().Author);
    }

    [Fact]
    async public void ReadDBWithLimitTest()
    {
        var db = CSVDatabase<Cheep>.Instance;
        db.SetPath("../../../../../data/TestDatabase.csv");

        db.Store(new Cheep("testuser", "testcheep", "1726835588"));
        db.Store(new Cheep("testuser", "testcheep", "1726835588"));
        db.Store(new Cheep("testuser", "testcheep", "1726835588"));
        File.Delete("../../../../../data/TestDatabase.csv");
        var cheeps = db.Read(2);
        Assert.Equal(2, cheeps.Count());
    }

    
    
}