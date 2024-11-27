using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;

public interface IAuthorRepository
{
    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task<Author> GetAuthorByNameEntity(string author);
    public Task AddFollows(string you, string me);
    public Task RemoveFollows(string you, string me);
    public Task<bool> ContainsFollower(string you, string me);
}