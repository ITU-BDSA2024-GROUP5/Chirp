using Chirp.Razor.Pages;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    
    public List<CheepViewModel> GetCertainCheeps(int page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    public List<CheepViewModel> GetCheeps()
    {
        return DBFacade.ReadDB();
    }
    
    public List<CheepViewModel> GetCertainCheeps(int page)
    {
        int cheepsPerPage = 10;
        List<CheepViewModel> cheepsForGivenPage = new List<CheepViewModel>();
        List<CheepViewModel> cheeps = GetCheeps();
        if (cheeps.Count > page * cheepsPerPage)
        {
            for (int i = cheepsPerPage * page; i < cheepsPerPage * (page + 1); i++)
            {
                cheepsForGivenPage.Add(cheeps[i]);
            }
            return cheepsForGivenPage;
        }
        else
        {
            return null;
        }
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return DBFacade.ReadDBByAuthor(author);
    }
}
