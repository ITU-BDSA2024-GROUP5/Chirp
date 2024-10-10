using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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
        _cheepServiceDB.CheckIfAuthorExists(author);
        if(Request.Query.ContainsKey("cheep")){
            _cheepServiceDB.Write(new Cheep() { Text = Request.Query["cheep"].ToString(), Author = new Author() { Name = author } });
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
