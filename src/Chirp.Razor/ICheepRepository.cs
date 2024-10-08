namespace Chirp.Razor.Pages;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);
    public void Write(Cheep cheep);
}