using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel 
{
    public List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;

    public UserTimelineModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        taskHandlerAsync(author);
        return Page();
    }

    public async Task taskHandlerAsync(string author){
        if(Request.Query["cheep"].ToString() != null){
            await _cheepRepository.Write(new Cheep() { Text = Request.Query["cheep"].ToString(), Author = await _cheepRepository.GetAuthorByName(author) });
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
