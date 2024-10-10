using Chirp.Razor.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor;

using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

class CheepServiceDB : ICheepServiceDB
{
    private readonly ICheepRepository _cheepRepository;

    private Author _author;
    public CheepServiceDB(ICheepRepository cheepRepository) {
        _cheepRepository = cheepRepository;
    }
    
    public void Write(Cheep cheep){
        CheckIfAuthorExists(cheep.Author.Name);
        CreateCheep(cheep);
    }

    private async void CreateCheep(Cheep cheep){
        Cheep newCheep = new Cheep();
        newCheep.CheepId = await _cheepRepository.GetHighestCheepId() + 1;
        newCheep.AuthorId = _author.AuthorId;
        newCheep.Text = cheep.Text;
        newCheep.TimeStamp = DateTime.Now;
        newCheep.Author = _author;
        await _cheepRepository.WriteCheep(newCheep);
    }


    private async void CreateAuthor(string author){
        Author newAuthor = new Author();
        newAuthor.Name = author;
        newAuthor.AuthorId = await _cheepRepository.GetHighestAuthorId() + 1;
        newAuthor.Email = author + "@chirp.com";
        newAuthor.Cheeps = new List<Cheep>();
        await _cheepRepository.WriteAuthor(newAuthor);
        _author = newAuthor;
    }


    private void CheckIfAuthorExists(string author){
        if(_cheepRepository.GetAuthorByName(author) == null) {
            CreateAuthor(author);
        } 
    }

    
}