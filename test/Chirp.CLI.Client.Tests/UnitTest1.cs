namespace Chirp.CLI.CLient.Test;

public class UnitTest1
{
    [Theory]
    [InlineData("1725526773", "05/09/2024 10.59.33")]
    [InlineData("1725527352", "05/09/2024 11.09.12")]
    [InlineData("1725527376", "05/09/2024 11.09.36")]
    [InlineData("1725792808", "08/09/2024 12.53.28")]
    public void TestUserInterfaceGetDateFormatted(string unixTimestamp, string expectedFormattedDate)
    {
        //Arrange
        //Act
        var actual = UserInterface.GetDateFormatted(unixTimestamp);
        //Assert
        Assert.Equal(actual, expectedFormattedDate);
    }
}