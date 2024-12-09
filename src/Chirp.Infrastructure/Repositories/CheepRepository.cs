using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;


/// <summary>
/// Repository for the Cheep table. Contains methods to interact with the Cheep table.
/// </summary>
public class CheepRepository : ICheepRepository
{
    private readonly ApplicationDbContext _context;

    public CheepRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided.
    /// </summary>
    /// <param name="page">Page number to read from.</param>
    /// <returns>List of CheepDTO</returns>
    public async Task<List<CheepDto>?> Read(int page)
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
        var cheeps = WrapInDto(result);
        return cheeps;
    }

    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided, by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from.</param>
    /// <param name="author">Author to read cheeps by.</param>
    /// <returns></returns>
    public async Task<List<CheepDto>?> ReadByAuthor(int page, string author)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDto(result);
        return cheeps;
    }
    
    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided, by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from</param>
    /// <param name="author">Author to read cheeps by</param>
    /// <returns>List of Cheeps</returns>
    public async Task<List<Cheep>?> ReadByAuthorEntity(int page, string author)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        return result;
    }
    
    /// <summary>
    /// Reads all cheeps by a specific author.
    /// </summary>
    /// <param name="author">The author to read cheeps by</param>
    /// <returns>List of CheepDTO</returns>
    public async Task<List<CheepDto>?> ReadAllCheeps(string author)
    {
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDto(result);
        return cheeps;
    }
    
    

    /// <summary>
    /// Function for finding the highest CheepId in the database.
    /// </summary>
    /// <returns>The highest CheepID in the DB</returns>
    public async Task<int> GetHighestCheepId(){
        var query = _context.Cheeps
            .Select(c => c)
            .OrderByDescending(c => c.CheepId);
        var result = await query.FirstOrDefaultAsync();
        return result?.CheepId ?? 0;
    }

    /// <summary>
    /// Writes a cheep to the database.
    /// </summary>
    /// <param name="cheep"></param>
    /// <returns></returns>
    public async Task WriteCheep(Cheep cheep)
    {
        await _context.Cheeps.AddAsync(cheep);
        cheep.Author.Cheeps.Add(cheep);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Gets all the cheeps written by a specific author.
    /// </summary>
    /// <param name="author"></param>
    /// <returns>List with CheepDTO's from a given user</returns>
    public async Task<List<CheepDto>?> GetCheepsByAuthor(string author)
    {
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp);
        
        var result = await query.ToListAsync();
        
        var cheeps = WrapInDto(result);

        return cheeps;
    }

    
    /// <summary>
    /// Gets cheeps by authors followed by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from</param>
    /// <param name="author">The author to read cheeps by</param>
    /// <param name="authors">List of authors that the author follows</param>
    /// <returns></returns>
    public async Task<List<CheepDto>?> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors)
    {
        var cheepsQuery = _context.Cheeps
            .Include(c => c.Author)
            .Where(c => c.Author.UserName != null && (c.Author.UserName == author || (authors != null && authors.Contains(c.Author.UserName))))
            .OrderByDescending(c => c.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);

        var cheeps = await cheepsQuery.ToListAsync();
        return WrapInDto(cheeps);
    }

    /// <summary>
    /// Wraps a list of Cheep objects in CheepDTO objects. 
    /// </summary>
    /// <param name="cheeps"></param>
    /// <returns></returns>
    public static List<CheepDto> WrapInDto(List<Cheep> cheeps)
    {
        var list = new List<CheepDto>();
        foreach (var cheep in cheeps)
        {
            if (cheep.Author.UserName != null)
                list.Add(new CheepDto
                {
                    Text = cheep.Text,
                    Author = cheep.Author.UserName,
                    TimeStamp = cheep.TimeStamp.ToString("R")
                });
        }
        return list;
    }
    
    /**
     * This method is used to sort and divide all the cheeps registered into 32 per page on the user's timeline.
     */
    public async Task<List<CheepDto>?> GetPaginatedResultByAuthor(int page, string author, int pageSize = 32)
    {
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var result = await query.ToListAsync();
        
        var cheeps = WrapInDto(result);

        return cheeps;
    }
    
    /**
     * This method is used to sort and divide all the cheeps registered into 32 per page on the public timeline.
     */
    public async Task<List<CheepDto>?> GetPaginatedResult(int page, int pageSize = 32)
    {
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        
        var result = await query.ToListAsync();
        
        var cheeps = WrapInDto(result);

        return cheeps;
    }

    public async Task<int> GetCheepsCountByFollows(string author, List<string>? authors)
    {
        var cheeps = new List<Cheep>();
        if (authors != null)
        {
            foreach (var auth in authors)
            {
                var query = _context.Cheeps
                    .Select(cheep => cheep)
                    .Include(c => c.Author)
                    .Where(cheep => cheep.Author.UserName == auth);
                // Execute the query and store the results
                var result = await query.ToListAsync();
                cheeps.AddRange(result);
            }
        }
        return cheeps.Count;
    }
    public async Task<int> GetCount()
    {
        var count = await _context.Cheeps.CountAsync();
        return count;
    }
}