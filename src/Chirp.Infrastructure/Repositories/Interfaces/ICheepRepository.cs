using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;


/// <summary>
/// Interface for the CheepRepository. Contains methods to interact with the Cheeps table.
/// </summary>
public interface ICheepRepository
{
    public Task<List<CheepDto>?> Read(int page);
    public Task<List<CheepDto>?> ReadByAuthor(int page, string author);
    public Task<List<CheepDto>?> ReadAllCheeps(string author);
    public Task<List<CheepDto>?> ReadAllCheeps();
    public Task<int> GetHighestCheepId();
    public Task WriteCheep(Cheep cheep);
    public Task<List<CheepDto>?> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors);
    public Task<List<CheepDto>?> GetPaginatedResult(int page, int pageSize);
    public Task<List<CheepDto>?> GetPaginatedResultByAuthor(int page, string author, int pageSize = 32);
    public Task<int> GetCount();
    public Task<int> GetCheepsCountByFollows(string author, List<string>? authors);
    public Task<List<CheepDto>?> GetCheepsByAuthor(string author);

}