using System.Runtime.InteropServices.JavaScript;
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

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

    public async Task<int> GetAuthorId(string author)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.UserName == author);
        var result = await query.FirstOrDefaultAsync();
        return result?.AuthorId ?? 0;
    }

    public async Task WriteAuthor(Author author)
    {
        var queryResult = await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }

    public static AuthorDTO WrapInDTO(Author author)
    {   
        if(author == null) return null;
        if (author.Follows == null) author.Follows = new List<string>();
        return new AuthorDTO{
            Name = author.UserName,
            Email = author.Email,
            Follows = author.Follows
        };
    }

    public async Task<List<string>> GetFollowers(string me)
    {
        var author = await _context.Authors
            .FirstAsync(a => a.UserName == me);
        
        if (author.Follows == null) return new List<string>();
        return author.Follows;
    }

    // to avoid ambiguity and confusion, 'you' is the user 'me' wants to follow
    public async Task AddFollower(string you, string me)
    {
        var author = await _context.Authors
            .SingleOrDefaultAsync(a => a.UserName == me);
        
        if (author.Follows == null) author.Follows = new List<string>();
        author.Follows.Add(you);
    }
    
    public async Task RemoveFollower(string you, string me)
    {
        var author = await _context.Authors
            .FirstAsync(a => a.UserName == me);
        
        author.Follows ??= new List<string>();
        author.Follows.Remove(you);
    }

    public async Task<bool> ContainsFollower(string you, string me)
    {
        var author = await _context.Authors
            .FirstAsync(a => a.UserName == me);
        return author.Follows != null && author.Follows.Contains(you);
    }
    
}
