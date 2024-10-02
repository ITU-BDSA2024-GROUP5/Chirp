namespace Chirp.Razor.Test;

using Microsoft.AspNetCore.Mvc.Testing;


public class UnitTests
{
    [Fact]
    public void SetPage_CheckEmptyPage()
    {
        var page = "";
        var cheepService = new CheepService();
        cheepService.setPage(page);
        Assert.Equal(1, cheepService.getPageNumber());
    }

    [Fact]
    public void SetPage_CheckNumberPage()
    {
        var page = "3";
        var cheepService = new CheepService();
        cheepService.setPage(page);
        Assert.Equal(3, cheepService.getPageNumber());
    }

    [Fact]
    public void GetCheeps_ReturnsListOfCheeps()
    {
        var cheepService = new CheepService();
        
    }
}