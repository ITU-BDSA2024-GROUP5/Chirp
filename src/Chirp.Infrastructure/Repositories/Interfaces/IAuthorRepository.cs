

using Chirp.Infrastructure.DataModels;
using Chirp.Core.DTO;

public interface IAuthorRepository
{
    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task WriteAuthor(Author author);
}