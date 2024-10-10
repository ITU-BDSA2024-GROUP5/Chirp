using Chirp.Razor.Pages;

namespace Chirp.Razor;

using Microsoft.EntityFrameworkCore;

class CheepServiceDB : ICheepServiceDB
{
    private readonly ICheepRepository CheepRepository;

    public CheepServiceDB(ICheepRepository cheepRepository){
        CheepRepository = cheepRepository;
    }

    public Cheep cheepCreator(Cheep cheep){
        Cheep newCheep = new Cheep();
        newCheep.Author = cheep.Author;

        return 
    }

    public void checkIfAuthorExists(Author author){
        CheepRepository.GetAuthorByName(author.Name);
    }

    public Author authorCreator(Author author){
        Author newAuthor = new Author();
        newAuthor.Name = author.Name;
        newAuthor.Email = author.Email;
        newAuthor.
    }
}