using Chirp.Web;

namespace Chirp.Razor.Test;

using Microsoft.AspNetCore.Mvc.Testing;

public class TestAPI : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TestAPI(WebApplicationFactory<Program> fixture)
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5177");
    }


    //Added test methods
    [Fact]
    public async Task CanSeePublicTimeline()
    {
        var response = await _client.GetAsync("/Public");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains("Public Timeline", content);
    }

    [Theory]
    [InlineData("Helge")]
    [InlineData("Adrian")]
    public async Task CanSeePrivateTimeline(string author)
    {
        var response = await _client.GetAsync($"/{author}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Chirp!", content);
        Assert.Contains($"{author}'s Timeline", content);
    }
}
