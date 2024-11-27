using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;

public interface ICheepRepository
{
    public Task<List<CheepDTO>> Read(int page);
    public Task<List<CheepDTO>> ReadByAuthor(int page, string author);
    public Task<List<CheepDTO>> ReadAllCheeps(string author);
    public Task<int> GetHighestCheepId();
    public Task WriteCheep(Cheep cheep);
    public Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors);
}