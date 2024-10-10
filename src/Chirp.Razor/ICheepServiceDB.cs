namespace Chirp.Razor.Pages;

public interface ICheepServiceDB
{
    public void Write(Cheep cheep);
    public void CheckIfAuthorExists(String author);
}