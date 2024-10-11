using Chirp.Razor.Pages;

namespace Chirp.Razor;

using Microsoft.EntityFrameworkCore;

public class CheepRepository : ICheepRepository
{
    private readonly CheepDBContext _context;

    public CheepRepository(CheepDBContext context)
    {
        _context = context;
        context.Database.EnsureCreated();
        DbInitializer.SeedDatabase(_context);
    }

    public async Task<List<CheepDTO>> Read(int page)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);

        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDTO(result);
        return cheeps;
    }

    public async Task<List<CheepDTO>> ReadByAuthor(int page, string author)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.Name == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDTO(result);
        return cheeps;
    }

    public async Task<int> GetHighestAuthorId()
    {
        var query = _context.Authors
            .Select(a => a)
            .OrderByDescending(a => a.AuthorId);
        var result = await query.FirstOrDefaultAsync();
        return result?.AuthorId ?? 0;
    }

    public async Task<int> GetHighestCheepId(){
        var query = _context.Cheeps
            .Select(c => c)
            .OrderByDescending(c => c.CheepId);
        var result = await query.FirstOrDefaultAsync();
        return result?.CheepId ?? 0;
    }

    public async Task WriteCheep(Cheep cheep)
    {
        var queryResult = await _context.Cheeps.AddAsync(cheep);
        await _context.SaveChangesAsync();
    }

    public async Task WriteAuthor(Author author){
        var queryResult = await _context.Authors.AddAsync(author);
        await _context.SaveChangesAsync();
    }

    public async Task<Author> GetAuthorByName(string author){
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.Name == author);
        var result = await query.FirstOrDefaultAsync();
        return result; 
    }


    public static List<CheepDTO> WrapInDTO(List<Cheep> cheeps)
    {
        var list = new List<CheepDTO>();
        foreach (var cheep in cheeps)
        {
            list.Add(new CheepDTO
            {
                Text = cheep.Text,
                Author = cheep.Author.Name,
                TimeStamp = cheep.TimeStamp.ToString()
            });
        }
        //return dto stuff
        return list;
    }

    
}