using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;

namespace Chirp.Infrastructure.Repositories.Interfaces;

/// <summary>
/// Interface for the AuthorRepository. Contains methods to interact with the Authors table.
/// </summary>
public interface IAuthorRepository
{
    public Task<AuthorDto?> GetAuthorByName(string author);
    public Task<AuthorDto?> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task<List<string>?> GetFollowed(string author);
    public Task<Author> GetAuthorByNameEntity(string author);
    public Task AddFollows(string you, string me);
    public Task RemoveFollows(string you, string me);
    public Task<bool> ContainsFollower(string you, string me);
}