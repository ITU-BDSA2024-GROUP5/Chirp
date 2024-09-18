using Chirp.CLI;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

// Create an HTTP client object
var baseURL = "http://localhost:5249";
using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.BaseAddress = new Uri(baseURL);

// Send an asynchronous HTTP GET request and automatically construct a Cheep object from the
// JSON object in the body of the response
var cheeps = await client.GetFromJsonAsync<List<Cheep>>("read");
foreach (var cheep in cheeps)
{
    UserInterface.PrintCheep(cheep);
}


public record Cheep(string Author, string Message, string Timestamp);