using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Repositories;

public class CheepRepository : ICheepRepository
{
    private readonly ApplicationDbContext _context;

    public CheepRepository(ApplicationDbContext context)
    {
        _context = context;
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
            .Where(cheep => cheep.Author.UserName == author)
            .OrderByDescending(cheep => cheep.TimeStamp)
            .Skip((page - 1) * 32)
            .Take(32);
        // Execute the query and store the results
        var result = await query.ToListAsync();
        var cheeps = WrapInDTO(result);
        return cheeps;
    }
    
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

    public async Task<int> GetHighestCheepId(){
        var query = _context.Cheeps
            .Select(c => c)
            .OrderByDescending(c => c.CheepId);
        var result = await query.FirstOrDefaultAsync();
        return result?.CheepId ?? 0;
    }

    public async Task WriteCheep(Cheep cheep)
    {
        await _context.Cheeps.AddAsync(cheep);
        cheep.Author.Cheeps.Add(cheep);
        await _context.SaveChangesAsync();
    }

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

    public static List<CheepDTO> WrapInDTO(List<Cheep> cheeps)
    {
        var list = new List<CheepDTO>();
        foreach (var cheep in cheeps)
        {
            list.Add(new CheepDTO
            {
                Text = cheep.Text,
                Author = cheep.Author.UserName,
                TimeStamp = cheep.TimeStamp.ToString()
            });
        }
        return list;
    }
}