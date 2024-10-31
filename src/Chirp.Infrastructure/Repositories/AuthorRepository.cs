using Oauth.Pages;

using Microsoft.EntityFrameworkCore;
using Oauth.Data;

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
            .Where(a => a.Name == author);
        var result = await query.FirstOrDefaultAsync();
        var Author = WrapInDTO(result);
        return Author;
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

    public async Task WriteAuthor(Author author)
    {
        var queryResult = await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }

    public static AuthorDTO WrapInDTO(Author author)
    {
        return new AuthorDTO{
            Name = author.Name,
            Email = author.Email
        };
    }
}
