using Chirp.Razor.Pages;

namespace Chirp.Razor;

using Microsoft.EntityFrameworkCore;

class CheepRepository : ICheepRepository
{
    private readonly CheepDBContext _context;

    private static CheepServiceDB CheepServiceDB = new CheepServiceDB();

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
            .OrderBy(cheep => cheep.TimeStamp)
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
            .OrderBy(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDTO(result);
        return cheeps;
    }

    public async Task Write(Cheep cheep)
    {
        
        Cheep newCheep = CheepServiceDB.cheepCreator(cheep);
        
        var queryResult = await _context.Cheeps.AddAsync(newCheep);
        
        await _context.SaveChangesAsync();
    }

    public async Task<AuthorDTO> GetAuthorByName(string author){
        var query = _context.Authors
            .Select(a => a)
            .Where(a => a.Name == author);
        var result = await query.FirstOrDefaultAsync();
        return result != null ? WrapAuthorInDTO(result) : null;
    }

    private static AuthorDTO WrapAuthorInDTO(Author author){
        AuthorDTO authorDTO = new AuthorDTO();
        authorDTO.Name = author.Name;
        authorDTO.Email = author.Email;
        authorDTO.AuthorId = author.AuthorId;
        return authorDTO;
    }

    private static List<CheepDTO> WrapInDTO(List<Cheep> cheeps)
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