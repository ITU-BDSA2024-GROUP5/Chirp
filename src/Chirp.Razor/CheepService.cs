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
    
    public int getPageNumber() { return this.page; } // is implemented for test purposes only
    
    public List<CheepViewModel> GetCheeps()
    {
        return DBFacade.ReadDB(this.page);
    }

    public List<CheepViewModel> GetCheepsFromAuthor(string author)
    {
        // filter by the provided author name
        return DBFacade.ReadDBByAuthor(this.page,author);
    }
}
