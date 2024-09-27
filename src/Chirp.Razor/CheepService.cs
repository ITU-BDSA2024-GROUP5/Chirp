using Chirp.Razor.Pages;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    public List<CheepViewModel> GetCheeps()
    {
        return DBFacade.ReadDB();
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return DBFacade.ReadDBByAuthor(author);
    }
}
