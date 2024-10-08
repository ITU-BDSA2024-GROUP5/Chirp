using Chirp.Razor.Pages;

namespace Chirp.Razor;

using Microsoft.EntityFrameworkCore;

class CheepRepository : ICheepRepository
{
    private readonly CheepDBContext _context;

    public CheepRepository(CheepDBContext context)
    {
        _context = context;
    }

    public async Task<List<Cheep>> Read(int page)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .OrderBy(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);

        // Execute the query and store the results
        var result = await query.ToListAsync();
        
            //dto stuff
            //return dto stuff
        return result;
    }

    public async Task<List<Cheep>> ReadByAuthor(int page, string author)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.Name == author)
            .OrderBy(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        return result;
    }

    public async void Write(Cheep cheep)
    {
        Cheep newCheep = new() { Text = cheep.Text, Author = cheep.Author, TimeStamp = cheep.TimeStamp };
        var queryResult = await _context.Cheeps.AddAsync(newCheep);

        await _context.SaveChangesAsync();
    }
}