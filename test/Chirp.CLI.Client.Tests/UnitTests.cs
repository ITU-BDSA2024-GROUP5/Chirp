using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chirp.CLI.CLient.Test;

using SimpleDB;
using Chirp.CLI.CLient;
using Xunit;


public class UnitTests

{
    [Fact]
    public void TestCsvDBSingletonPattern()
    {
        var db1 = CSVDatabase<Cheep>.Instance;
        var db2 = CSVDatabase<Cheep>.Instance;
        
        Assert.Equal(db1, db2);
    }

    [Theory]
    [InlineData("1726065879", "11/09/2024 16:44:39")]
    public void TestUserInterfaceGetDateFormatted(string unixTimestamp, string expectedFormattedDate)
    {
        //Arrange aa
        //Act
        var actual = UserInterface.GetDateFormatted(unixTimestamp).ToString();
        //Assert
        Assert.Equal(expectedFormattedDate, actual);
    }

    [Fact]
    public void TestPrintCheep()
    {
        //Arrange
        var cheep = new Cheep("salj", "hej med dig", "11/09/2024 16.44.39");
        string author = cheep.Author;
        var message = cheep.Message;
        var timestamp = UserInterface.GetDateFormatted(cheep.Timestamp);
        var expectedOutput = author + " @ " + timestamp + ": " + message; // expected output
        
        
        //Assert
        Assert.Equal(expectedOutput, UserInterface.formatCheep(cheep)); 
        
    }
}