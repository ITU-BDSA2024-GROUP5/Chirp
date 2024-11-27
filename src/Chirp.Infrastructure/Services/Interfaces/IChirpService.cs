using Chirp.Infrastructure.Data.DTO;

namespace Chirp.Infrastructure.Services.Interfaces;

public interface IChirpService
{
    public Task CreateCheep(string name, string text);
    public Task<AuthorDTO> GetAuthorByName(string author);
    public Task<AuthorDTO> GetAuthorByEmail(string email);
    public Task<int> GetHighestAuthorId();
    public Task<bool> ContainsFollower(string you, string me);
    public Task AddFollows(string you, string me);
    public Task RemoveFollows(string you, string me);
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);
    public Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors);
}