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

    public async Task<Author> GetAuthorByName(string author)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.Name == author);
        var result = await query.FirstOrDefaultAsync();
        return result;
    }
    
    public async Task<Author> GetAuthorByEmail(string email)
    {
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.Email == email);
        var result = await query.FirstOrDefaultAsync();
        return result;
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

    public static List<AuthorDTO> WrapInDTO(List<Author> authors)
    {
        var list = new List<AuthorDTO>();
        foreach (var author in authors)
        {
            list.Add(new AuthorDTO
            {
                Name = author.Name,
                Email = author.Email
            });
        }
        return list;
    }
}
