using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;


/// <summary>
/// Repository for the Author table. Contains methods to interact with the Authors table.
/// </summary>
public class AuthorRepository : IAuthorRepository
{
    private readonly ApplicationDbContext _context;

    public AuthorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuthorDTO> GetAuthorByName(string author)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.UserName == author);
        var result = await query.FirstOrDefaultAsync();
        var Author = WrapInDTO(result);
        return Author;
    }
    
    /// <summary>
    /// Returns an author entity and not an AuthorDTO. This could be fixed.
    /// </summary>
    /// <param name="author">The author to find by name</param>
    /// <returns>Author entity</returns>
    public async Task<Author> GetAuthorByNameEntity(string author)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.UserName == author);
        var result = await query.FirstOrDefaultAsync();
        return result;
    }
    
    public async Task<AuthorDTO> GetAuthorByEmail(string email)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.Email == email);
        var result = await query.FirstOrDefaultAsync();
        var author = WrapInDTO(result);
        return author;
    }

    public async Task<int> GetHighestAuthorId()
    {
        var query = _context.Authors
            .Select(a => a)
            .OrderByDescending(a => a.AuthorId);
        var result = await query.FirstOrDefaultAsync();
        return result?.AuthorId ?? 0;
    }
    
    /// <summary>
    /// Makes one author follow another author.
    /// </summary>
    /// <param name="you">The author that wants to follow another author.</param>
    /// <param name="me">The author to follow</param>
    public async Task AddFollows(string you, string me)
    {
        var authordto = await GetAuthorByName(you);
        var author = _context.Authors.First(a => a.UserName == authordto.Name);
        if (author.Follows == null)
        {
            author.Follows = new List<string>();
        }
        author.Follows.Add(me);
        await _context.SaveChangesAsync();
    }
    
    
    /// <summary>
    /// Makes one author un-follow another author.
    /// </summary>
    /// <param name="you">The author that wants to un-follow another author.</param>
    /// <param name="me">The author to un-follow</param>
    public async Task RemoveFollows(string you, string me)
    {
        var authordto = await GetAuthorByName(you);
        var author = _context.Authors.First(a => a.UserName == authordto.Name);
        if (author.Follows == null)
        {
            author.Follows = new List<string>();
        }
        author.Follows.Remove(me);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a given user follows another user.
    /// </summary>
    /// <param name="you">The author to check is followed.</param>
    /// <param name="me">The author to check if following.</param>
    /// <returns></returns>
    public async Task<bool> ContainsFollower(string you, string me)
    {
        var author = await _context.Authors
            .FirstAsync(a => a.UserName == me);
        return author.Follows != null && author.Follows.Contains(you);
    }
    
    private static AuthorDTO WrapInDTO(Author author)
    {   
        if(author == null) return null;
        if (author.Follows == null) author.Follows = new List<string>();
        return new AuthorDTO{
            Name = author.UserName,
            Email = author.Email,
            Follows = author.Follows
        };
    }
    
    
    // for test
    public async Task WriteAuthor(Author author)
    {
        var queryResult = await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }
}
