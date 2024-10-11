using System.Runtime.CompilerServices;
using Chirp.Razor.Pages;

namespace Chirp.Razor;

public class CheepServiceDB : ICheepServiceDB
{
    private readonly ICheepRepository _cheepRepository;

    private Author? _author;
    public CheepServiceDB(ICheepRepository cheepRepository) {
        _cheepRepository = cheepRepository;
    }
    
    public void Write(Cheep cheep) {
        CreateCheep(cheep);
    }

    private async void CreateCheep(Cheep cheep){
        Cheep newCheep = new Cheep()
        {
            CheepId = await _cheepRepository.GetHighestCheepId() + 1,
            Text = cheep.Text,
            TimeStamp = DateTime.Now,
            Author = _author,
            AuthorId = _author.AuthorId,
        };
        await _cheepRepository.WriteCheep(newCheep);
    }
    
    public async void CreateAuthor(string author){
        Author newAuthor = new Author()
        {
            Name = author,
            AuthorId = await _cheepRepository.GetHighestAuthorId() + 1,
            Email = author + "@chirp.com",
            Cheeps = new List<Cheep>()
        };
        _author = newAuthor;
        await _cheepRepository.WriteAuthor(newAuthor);
        
    }

    public async void CheckIfAuthorExists(string author){
        _author = await _cheepRepository.GetAuthorByName(author);
    
        if(_author == null){ 
            CreateAuthor(author);
        } 
        _author = await _cheepRepository.GetAuthorByName(author);
    }
}