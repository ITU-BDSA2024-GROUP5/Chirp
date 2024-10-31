
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;

using System.Runtime.CompilerServices;

public class CheepServiceDB : ICheepServiceDB
{
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    
    public CheepServiceDB(ICheepRepository cheepRepository, IAuthorRepository authorRepository) {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }
    
    public async Task WriteAuthor(Author author)
    {
        await _authorRepository.WriteAuthor(author);
    }
    
    public async Task WriteCheep(Cheep cheep)
    {
        await _cheepRepository.WriteCheep(cheep);
    }

    public async Task<Author> CreateAuthor(string author){
        Author newAuthor = new Author()
        {
            Name = author,
            AuthorId = await _authorRepository.GetHighestAuthorId() + 1,
            Email = author + "@chirp.com",
            Cheeps = new List<Cheep>()
        };
        return newAuthor;
    }

    public async Task<Cheep> CreateCheep(Author author, string text)
    {
        var cheep = new Cheep()
        {
            CheepId = await _cheepRepository.GetHighestCheepId() + 1,
            Text = text,
            TimeStamp = DateTime.Now,
            Author = author,
            AuthorId = author.AuthorId
        };
        return cheep;
    }

    public async Task<bool> CheckIfAuthorExists(string author){
        var checkauthor = await _authorRepository.GetAuthorByName(author);
    
        if(checkauthor == null)
        {
            return false;
        }

        return true;
    }
}