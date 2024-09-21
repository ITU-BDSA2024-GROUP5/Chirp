namespace Chirp.CLI.CLient.Test;


using SimpleDB;


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
}