
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;

public interface ICheepServiceDB
{
    public Task WriteCheep(Cheep cheep);
    public Task WriteAuthor(Author author);
    public Task<bool> CheckIfAuthorExists(String author);
    public Task<AuthorDTO> CreateAuthor(string authorName);
    public Task<Cheep> CreateCheep(string name, string text);
    public Task<AuthorDTO> GetAuthorByString(string author);
}