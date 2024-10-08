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
        Cheeps = await _cheepRepository.ReadByAuthor(getPage(),author);
        return Page();
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
