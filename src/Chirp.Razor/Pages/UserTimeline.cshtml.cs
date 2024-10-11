using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel 
{
    public List<CheepDTO> Cheeps { get; set; }
    
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
        var authorExists = await _cheepServiceDB.CheckIfAuthorExists(author);
        if (!authorExists)
        {
            createdAuthor = await _cheepServiceDB.CreateAuthor(author);
            await _cheepServiceDB.WriteAuthor(createdAuthor);
        }
        else
        {
            createdAuthor = await _cheepRepository.GetAuthorByName(author);
        }
        if(Request.Query.ContainsKey("cheep"))
        {
            var text = Request.Query["cheep"].ToString();
            var cheep = await _cheepServiceDB.CreateCheep(createdAuthor, text);
            await _cheepServiceDB.WriteCheep(cheep);;
        } 
        Cheeps = await _cheepRepository.ReadByAuthor(getPage(), author);
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
