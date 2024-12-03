using Chirp.Infrastructure.Data.DTO;

namespace Chirp.Infrastructure.Services.Interfaces;


/// <summary>
/// Service Interface that defines methods that provide access to cheeps and authors.
/// </summary>
public interface IChirpService
{
    public Task CreateCheep(string name, string text);
    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task<bool> ContainsFollower(string you, string me);
    public Task AddFollows(string you, string me);
    public Task RemoveFollows(string you, string me);
    public Task<List<CheepDTO>> ReadAllCheeps(string author);
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);
    public Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors);
    public Task<List<string>> GetFollowed(string author);
    public Task<List<CheepDTO>> GetPaginatedResult(int page, int pageSize = 32);
    public Task<int> GetCount();
    public Task<int> GetCheepsCountByFollows(string author, List<string>? authors);
    public Task<List<CheepDTO>> GetCheepsByAuthor(string author);
    public Task<List<CheepDTO>> GetPaginatedResultByAuthor(int page, string author, int pageSize = 32);
}