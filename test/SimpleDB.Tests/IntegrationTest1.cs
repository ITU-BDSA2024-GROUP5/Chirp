namespace SimpleDB.Tests;

using CsvHelper;
using System.Globalization;
using SimpleDB;
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
    public void ReadWriteDBTest ()
    {
        //arrange
        ArrangeTestDatabase();
        var db = new CSVDatabase<Cheep>();
        db.SetPath("../../../../../data/TestDatabase.csv");
        
        //act
        db.Store(new Cheep("hej", "hej2", "hej3"));
        var storedCheep = db.Read().Last();

        //assert
        Assert.Equal("hej", storedCheep.Author);
        Assert.Equal("hej2", storedCheep.Message);
        Assert.Equal("hej3", storedCheep.Timestamp);

        File.Delete("../../../../../data/TestDatabase.csv");

    }
}