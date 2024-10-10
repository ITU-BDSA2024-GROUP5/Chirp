namespace Chirp.Razor.Pages;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);

    public Task<Author> GetAuthorByName(string author);

    public Task<int> GetHighestAuthorId();

    public Task<int> GetHighestCheepId();
    public Task WriteCheep(Cheep cheep);

    public Task WriteAuthor(Author author);
}