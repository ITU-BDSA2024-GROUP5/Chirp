
using Chirp.Core.DataModels;

public interface ICheepServiceDB
{
    public Task WriteCheep(Cheep cheep);
    public Task WriteAuthor(Author author);
    public Task<bool> CheckIfAuthorExists(String author);
    public Task<Author> CreateAuthor(string author);
    public Task<Cheep> CreateCheep(Author author, string text);
    public Task<Author> GetAuthorByString(string author);
}