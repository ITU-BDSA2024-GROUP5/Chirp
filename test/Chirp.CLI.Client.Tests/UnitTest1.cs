namespace Chirp.CLI.CLient.Test;

public class UnitTest1
{
    [Theory]
    [InlineData("1726065879", "11/09/2024 16:44:39")]
    public void TestUserInterfaceGetDateFormatted(string unixTimestamp, string expectedFormattedDate)
    {
        //Arrange
        //Act
        var actual = UserInterface.GetDateFormatted(unixTimestamp);
        //Assert
        Assert.Equal(actual, expectedFormattedDate);
    }
}