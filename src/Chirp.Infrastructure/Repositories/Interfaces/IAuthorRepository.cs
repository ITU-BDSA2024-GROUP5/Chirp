

using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;

public interface IAuthorRepository
{
    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task WriteAuthor(Author author);
}