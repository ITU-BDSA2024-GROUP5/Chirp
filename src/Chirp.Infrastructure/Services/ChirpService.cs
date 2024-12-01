using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;

namespace Chirp.Infrastructure.Services;

public class ChirpService : IChirpService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    
    public ChirpService(ICheepRepository cheepRepository, IAuthorRepository authorRepository) {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task CreateCheep(string name, string text)
    {
        var author = await GetAuthorByName(name);
        
        var cheep = new Cheep()
        {
            CheepId = await _cheepRepository.GetHighestCheepId() + 1,
            Text = text,
            TimeStamp = DateTime.Now,
            Author = await _authorRepository.GetAuthorByNameEntity(author.Name) // fix? repositories should only return dtos

        };
        await _cheepRepository.WriteCheep(cheep);
    }
    
    public async Task<AuthorDTO> GetAuthorByName(string author)
    {
        return await _authorRepository.GetAuthorByName(author);
    }

    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        return await _authorRepository.GetAuthorByEmail(email);
    }

    public async Task<int> GetHighestAuthorId()
    {
        return await _authorRepository.GetHighestAuthorId();
    }

    public async Task<bool> ContainsFollower(string you, string me)
    {
        return await _authorRepository.ContainsFollower(you, me);
    }

    public async Task AddFollows(string you, string me)
    {
        await _authorRepository.AddFollows(you, me);
    }

    public async Task RemoveFollows(string you, string me)
    {
        await _authorRepository.RemoveFollows(you, me);
    }

    public Task<List<CheepDTO>> ReadAllCheeps(string author)
    {
        return _cheepRepository.ReadAllCheeps(author);
    }
    
    public Task<List<CheepDTO>> Read(int page)
    {
        return _cheepRepository.Read(page);
    }
     
    public async Task<List<CheepDTO>> ReadByAuthor(int page, string author)
    {
        return await _cheepRepository.ReadByAuthor(page, author);
    }

    public Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors)
    {
        return _cheepRepository.GetCheepsFollowedByAuthor(page, author, authors);
    }
}