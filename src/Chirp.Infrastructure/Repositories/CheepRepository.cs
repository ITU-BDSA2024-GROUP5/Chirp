using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
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

    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided, by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from.</param>
    /// <param name="author">Author to read cheeps by.</param>
    /// <returns></returns>
    public async Task<List<CheepDTO>> ReadByAuthor(int page, string author)
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
        var cheeps = WrapInDTO(result);
        return cheeps;
    }
    
    
    /// <summary>
    /// Reads 32 cheeps from the database, starting from the page number provided, by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from</param>
    /// <param name="author">Author to read cheeps by</param>
    /// <returns>List of Cheeps</returns>
    private async Task<List<Cheep>> ReadByAuthorEntity(int page, string author)
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
    /// Reads 32 cheeps from the database, starting from the page number provided, by a specific author.
    /// Finds the author by email.
    /// </summary>
    /// <param name="page"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    public async Task<List<CheepDTO>> ReadByEmail(int page, string email)
    {
        // Define the query - with our setup, EF Core translates this to an SQLite query in the background
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.Email == email)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDTO(result);
        return cheeps;
    }
    
    
    /// <summary>
    /// Reads all cheeps by a specific author.
    /// </summary>
    /// <param name="author">The author to read cheeps by</param>
    /// <returns>List of CheepDTO</returns>
    public async Task<List<CheepDTO>> ReadAllCheeps(string author)
    {
        var query = _context.Cheeps
            .Select(cheep => cheep)
            .Include(c => c.Author)
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDTO(result);
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
    public async Task<List<CheepDTO>> GetCheepsByAuthor(string author)
    {
        var auth = _context.Users.FirstOrDefault(a => a.UserName == author);
        if (auth == null) return new List<CheepDTO>();
        if (auth.UserName == null) return new List<CheepDTO>();
        
        var cheeps = await ReadAllCheeps(auth.UserName);
        return cheeps;
    }

    
    /// <summary>
    /// Gets cheeps by authors followed by a specific author.
    /// </summary>
    /// <param name="page">Page number to read from</param>
    /// <param name="author">The author to read cheeps by</param>
    /// <param name="authors">List of authors that the author follows</param>
    /// <returns></returns>
    public async Task<List<CheepDTO>> GetCheepsFollowedByAuthor(int page, string author, List<string>? authors)
    {
        var cheeps = new List<Cheep>();
        if (authors != null)
        {
            foreach (var auth in authors)
            {
                var query = _context.Cheeps
                    .Select(cheep => cheep)
                    .Include(c => c.Author)
                    .Where(cheep => cheep.Author.UserName == auth)
                    .OrderByDescending(cheep => cheep.TimeStamp)
                    .Skip((page - 1) * 32)
                    .Take(32);
                // Execute the query and store the results
                var result = await query.ToListAsync();
                cheeps.AddRange(result);
            }
        }
        
        var authorCheeps = await ReadByAuthorEntity(page, author);
        cheeps.AddRange(authorCheeps);
        cheeps = cheeps.OrderByDescending(c => c.TimeStamp).ToList();
        cheeps = cheeps.Take(32).ToList();
        var cheepsDTO = WrapInDTO(cheeps);

        return cheepsDTO;
    }

    /// <summary>
    /// Wraps a list of Cheep objects in CheepDTO objects. 
    /// </summary>
    /// <param name="cheeps"></param>
    /// <returns></returns>
    public static List<CheepDTO> WrapInDTO(List<Cheep> cheeps)
    {
        var list = new List<CheepDTO>();
        foreach (var cheep in cheeps)
        {
            list.Add(new CheepDTO
            {
                Text = cheep.Text,
                Author = cheep.Author.UserName,
                TimeStamp = cheep.TimeStamp.ToString("R")
            });
        }
        return list;
    }
}