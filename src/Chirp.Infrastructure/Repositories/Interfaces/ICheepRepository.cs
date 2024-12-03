using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;


/// <summary>
/// Interface for the CheepRepository. Contains methods to interact with the Cheeps table.
/// </summary>
public interface ICheepRepository
{
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);
    public Task<List<CheepDTO>> ReadAllCheeps(string author);
    public Task<int> GetHighestCheepId();
    public Task WriteCheep(Cheep cheep);
    public Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors);
    public Task<List<CheepDTO>> GetPaginatedResult(int page, int pageSize = 32);
    public Task<List<CheepDTO>> GetPaginatedResultByAuthor(int page, string author, int pageSize = 32);
    public Task<int> GetCount();
    public Task<int> GetCheepsCountByFollows(string author, List<string>? authors);
    public Task<List<CheepDTO>> GetCheepsByAuthor(string author);

}