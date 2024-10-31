using System.Runtime.CompilerServices;
using Oauth.Pages;

public class CheepServiceDB : ICheepServiceDB
{
    private readonly ICheepRepository _cheepRepository;
    
    public CheepServiceDB(ICheepRepository cheepRepository) {
        _cheepRepository = cheepRepository;
    }
    
    public async Task WriteAuthor(Author author)
    {
        await _cheepRepository.WriteAuthor(author);
    }
    
    public async Task WriteCheep(Cheep cheep)
    {
        await _cheepRepository.WriteCheep(cheep);
    }

    public async Task<Author> CreateAuthor(string author){
        Author newAuthor = new Author()
        {
            Name = author,
            AuthorId = await _cheepRepository.GetHighestAuthorId() + 1,
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

    public async Task<Author> GetAuthorByString(string author)
    {
        if (author.Contains('@'))
        {
            return await _cheepRepository.GetAuthorByEmail(author);
        }
        return await _cheepRepository.GetAuthorByName(author);
    }

    public async Task<bool> CheckIfAuthorExists(string author){
        var checkauthor = await _cheepRepository.GetAuthorByName(author);
    
        if(checkauthor == null)
        {
            return false;
        }

        return true;
    }
}