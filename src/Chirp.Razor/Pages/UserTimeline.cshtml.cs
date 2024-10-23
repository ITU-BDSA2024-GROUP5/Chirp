using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel 
{
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;
    private readonly ICheepServiceDB _cheepServiceDB;
    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
        _cheepServiceDB = new CheepServiceDB(cheepRepository);
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        await taskHandlerAsync(author);
        return Page();
    }

    public async Task taskHandlerAsync(string author)
    {
        Author createdAuthor;
        var isEmail = false;
        
        if (author.Contains('@'))
        {
            isEmail = true;
            createdAuthor = await _cheepRepository.GetAuthorByEmail(author);
        }
        else
        {
            createdAuthor = await _cheepRepository.GetAuthorByName(author);
        }
        
        if (createdAuthor == null)
        {
            createdAuthor = await _cheepServiceDB.CreateAuthor(author);
            await _cheepServiceDB.WriteAuthor(createdAuthor);
        }
        var text = Request.Query["cheep"].ToString();
        if (text.Length <= 160 && text.Length > 0)
        {
            var cheep = await _cheepServiceDB.CreateCheep(createdAuthor, text);
            await _cheepServiceDB.WriteCheep(cheep);
        }

        await fetchCheeps(author, isEmail);
    }

    public async Task fetchCheeps(string author, bool isEmail)
    {
        if (isEmail)
        {
            Cheeps = await _cheepRepository.ReadByEmail(getPage(), author);
        }
        else
        {
            Cheeps = await _cheepRepository.ReadByAuthor(getPage(), author);
        }
    }

    
    public int getPage()
    {
        try
        {
            return int.Parse(Request.Query["page"].ToString());
        }
        catch (Exception _)
        {
            return 0;
        }
    }
}
