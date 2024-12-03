using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;


/// <summary>
/// Interface for the AuthorRepository. Contains methods to interact with the Authors table.
/// </summary>
public interface IAuthorRepository
{
    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task<Author> GetAuthorByNameEntity(string author);

    public Task<List<string>> GetFollowed(string author);
    public Task AddFollows(string you, string me);
    public Task RemoveFollows(string you, string me);
    public Task<bool> ContainsFollower(string you, string me);
}