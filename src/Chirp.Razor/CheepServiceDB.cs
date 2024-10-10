using Chirp.Razor.Pages;


namespace Chirp.Razor;

class CheepServiceDB : ICheepServiceDB
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
        Cheep newCheep = new Cheep();
        newCheep.CheepId = await _cheepRepository.GetHighestCheepId() + 1;
        newCheep.Text = cheep.Text;
        newCheep.TimeStamp = DateTime.Now;
        newCheep.Author = _author;
        newCheep.AuthorId = _author.AuthorId;
        await _cheepRepository.WriteCheep(newCheep);
    }


    private async void CreateAuthor(string author){
        Author newAuthor = new Author();
        newAuthor.Name = author;
        newAuthor.AuthorId = await _cheepRepository.GetHighestAuthorId() + 1;
        newAuthor.Email = author + "@chirp.com";
        newAuthor.Cheeps = new List<Cheep>();
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