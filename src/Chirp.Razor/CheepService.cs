using Chirp.Razor.Pages;

public record CheepViewModel(string Author, string Message, string Timestamp);

public interface ICheepService
{
    public List<CheepViewModel> GetCheeps();
    public void setPage(String page);
    public List<CheepViewModel> GetCheepsFromAuthor(string author);
}

public class CheepService : ICheepService
{
    private int page = 1;

    public void setPage(String page)
    {
		if(page != ""){
		this.page=int.Parse(page);
		} else {
		this.page = 1;
		}
    }
    
    public List<CheepViewModel> GetCheeps()
    {
        int cheepsPerPage = 10;
        List<CheepViewModel> cheepsForGivenPage = new List<CheepViewModel>();
        List<CheepViewModel> cheeps = DBFacade.ReadDB();
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
