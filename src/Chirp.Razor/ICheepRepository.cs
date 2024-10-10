namespace Chirp.Razor.Pages;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);

    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task Write(Cheep cheep);
}