using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;

namespace Chirp.Infrastructure.Services;


/// <summary>
/// Service that provides access to cheeps and authors.
/// </summary>
public class ChirpService : IChirpService
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    
    public ChirpService(ICheepRepository cheepRepository, IAuthorRepository authorRepository) {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    
    /// <summary>
    /// Creates a new cheep.
    /// </summary>
    /// <param name="name">Author name to link cheep to</param>
    /// <param name="text">Cheep text</param>
    public async Task CreateCheep(string name, string text)
    {
        var author = await GetAuthorByName(name);
        if (author == null) return;
        if (author.Name == null) return; 

        var intendedAuthorName = await _authorRepository.GetAuthorByNameEntity(author.Name); // fix? repositories should only return dtos
        if (intendedAuthorName == null) return;
        
        var cheep = new Cheep()
        {
            CheepId = await _cheepRepository.GetHighestCheepId() + 1,
            Text = text,
            TimeStamp = DateTime.Now,
            Author = intendedAuthorName
        };
        
        await _cheepRepository.WriteCheep(cheep);
    }
    
    
    /// <summary>
    /// Gets an author by name.
    /// </summary>
    /// <param name="author">Author name to find author by</param>
    /// <returns>AuthorDTO</returns>
    public async Task<AuthorDTO?> GetAuthorByName(string author)
    {
        return await _authorRepository.GetAuthorByName(author);
    }

    
    /// <summary>
    /// Gets an author by email.
    /// </summary>
    /// <param name="email">Email to find author by</param>
    /// <returns>AuthorDTO</returns>
    public async Task<AuthorDTO?> GetAuthorByEmail(string email)
    {
        return await _authorRepository.GetAuthorByEmail(email);
    }

    
    /// <summary>
    /// Gets the highest AuthorId in the database. Used for creating new authors.
    /// </summary>
    /// <returns>Integer of highest authorid</returns>
    public async Task<int> GetHighestAuthorId()
    {
        return await _authorRepository.GetHighestAuthorId();
    }

    /// <summary>
    /// Checks if a given user follows another user.
    /// </summary>
    /// <param name="you">The author to check is followed.</param>
    /// <param name="me">The author to check if following.</param>
    /// <returns></returns>
    public async Task<bool> ContainsFollower(string you, string me)
    {
        return await _authorRepository.ContainsFollower(you, me);
    }

    
    /// <summary>
    /// Makes one author follow another author.
    /// </summary>
    /// <param name="you">The author that wants to follow another author.</param>
    /// <param name="me">The author to follow</param>
    public async Task AddFollows(string you, string me)
    {
        await _authorRepository.AddFollows(you, me);
    }

    
    /// <summary>
    /// Makes one author un-follow another author.
    /// </summary>
    /// <param name="you">The author that wants to un-follow another author.</param>
    /// <param name="me">The author to un-follow</param>
    public async Task RemoveFollows(string you, string me)
    {
        await _authorRepository.RemoveFollows(you, me);
    }

    
    /// <summary>
    /// Reads all cheeps by a specific author.
    /// </summary>
    /// <param name="author">The author to read cheeps by</param>
    /// <returns>List of CheepDTO</returns>
    public Task<List<CheepDTO>> ReadAllCheeps(string author)
    {
        return _cheepRepository.ReadAllCheeps(author);
    }
    
    public Task<List<CheepDTO>> ReadAllCheeps()
    {
        return _cheepRepository.ReadAllCheeps();
    }
    
    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided.
    /// </summary>
    /// <param name="page">Page number to read from.</param>
    /// <returns>List of CheepDTO</returns>
    public Task<List<CheepDTO>> Read(int page)
    {
        return _cheepRepository.Read(page);
    }
    
    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided, by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from.</param>
    /// <param name="author">Author to read cheeps by.</param>
    /// <returns></returns>
     
    public async Task<List<CheepDTO>> ReadByAuthor(int page, string author)
    {
        return await _cheepRepository.ReadByAuthor(page, author);
    }

    /// <summary>
    /// Gets a list of followers for a specific author.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public Task<List<string>> GetFollowed(string author)
    {
        return _authorRepository.GetFollowed(author);
    }

    
    /// <summary>
    /// Gets cheeps by authors followed by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from</param>
    /// <param name="author">The author to read cheeps by</param>
    /// <param name="authors">List of authors that the author follows</param>
    /// <returns></returns>
    public Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors)
    {
        return _cheepRepository.GetCheepsFollowedByAuthor(page, author, authors);
    }
    
    public Task<List<CheepDTO>> GetPaginatedResult(int page, int pageSize = 32)
    {
        return _cheepRepository.GetPaginatedResult(page, pageSize);
    }

    public Task<int> GetCheepsCountByFollows(string author, List<string>? authors)
    {
        return _cheepRepository.GetCheepsCountByFollows(author, authors);
    }
    public Task<int> GetCount()
    {
        return _cheepRepository.GetCount();
    }
    public Task<List<CheepDTO>> GetCheepsByAuthor(string author)
    {
        return _cheepRepository.GetCheepsByAuthor(author);
    }

    public Task<List<CheepDTO>> GetPaginatedResultByAuthor(int page, string author, int pageSize = 32)
    {
        return _cheepRepository.GetPaginatedResultByAuthor(page, author, pageSize);
    }
}