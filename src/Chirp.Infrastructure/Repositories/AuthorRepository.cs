using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Repositories.Interfaces;
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

    /// <summary>
    /// Function for getting an author by name.
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns>AuthorDTO</returns>
    public async Task<AuthorDto?> GetAuthorByName(string authorName)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.UserName != null && a.UserName.ToLower() == authorName.ToLower());
        var result = await query.FirstOrDefaultAsync();

        if (result == null) return null;
        
        var author = WrapInDto(result);
        return author;
    }
    
    /// <summary>
    /// Returns an author entity and not an AuthorDTO. This could be fixed.
    /// </summary>
    /// <param name="authorName">The author to find by name</param>
    /// <returns>Author entity</returns>
    public async Task<Author> GetAuthorByNameEntity(string authorName)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.UserName == authorName);
        var result = await query.FirstAsync();
        
        return result;
    }
    
    /// <summary>
    /// Function for getting/finding an author by email.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<AuthorDto?> GetAuthorByEmail(string email)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.Email != null && a.Email.Equals(email));
        var result = await query.FirstOrDefaultAsync();
        
        if (result == null) return null;
        
        var author = WrapInDto(result);
        return author;
    }

    /// <summary>
    /// Function for getting the highest AuthorId in the database.
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetHighestAuthorId()
    {
        var query = _context.Authors
            .Select(a => a)
            .OrderByDescending(a => a.AuthorId);
        var result = await query.FirstOrDefaultAsync();
        return result?.AuthorId ?? 0;
    }
    
    
    /// <summary>
    /// Function for getting the AuthorId by the name of the author.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public async Task<int> GetAuthorId(string author)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.UserName == author);
        var result = await query.FirstOrDefaultAsync();
        return result?.AuthorId ?? 0;
    }

    /// <summary>
    /// Writes an author to the database.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    public async Task WriteAuthor(Author author)
    {
        await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Wraps an Author entity in an AuthorDTO.
    /// </summary>
    /// <param name="author"></param>
    /// <returns></returns>
    private static AuthorDto? WrapInDto(Author author)
    {
        AuthorDto authorDto;

        if (author.UserName != null && author.Email != null)
        {
            authorDto = new AuthorDto()
            {
                Name = author.UserName,
                Email = author.Email,
                Follows = author.Follows
            };
            return authorDto;
        }

        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="me"></param>
    /// <returns></returns>
    public async Task<List<string>> GetFollowers(string me)
    {
        var author = await _context.Authors
            .FirstAsync(a => a.UserName == me);
        
        return author.Follows;
    }
    /// <summary>
    /// Makes one author follow another author.
    /// </summary>
    /// <param name="you">The author that wants to follow another author.</param>
    /// <param name="me">The author to follow</param>
    public async Task AddFollows(string you, string me)
    {
        var authorDto = await GetAuthorByName(you);
        
        if (authorDto == null) return;
        
        var author = _context.Authors.First(a => a.UserName == authorDto.Name);
        
        author.Follows.Add(me.ToLower());
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
        
        if (authordto == null) return;
        
        var author = _context.Authors.First(a => a.UserName == authordto.Name);

        author.Follows.Remove(me.ToLower());
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
        return author.Follows.Contains(you);
    }

    
    /// <summary>
    /// Returns a list of authors that a given author follows.
    /// </summary>
    /// <param name="authorName"></param>
    /// <returns>List of authors names</returns>
    public async Task<List<string>?> GetFollowed(string authorName)
    {
        var author = await _context.Authors
            .FirstAsync(a => a.UserName == authorName);
        return author.Follows;
    }
    
}
