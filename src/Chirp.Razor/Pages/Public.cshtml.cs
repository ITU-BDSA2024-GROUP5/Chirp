using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic.CompilerServices;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;

    public PublicModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }

    public async Task<ActionResult> OnGet()
    {
        Cheeps = await _cheepRepository.Read(parsePage(Request.Query["page"].ToString()));
        return Page();
    }

    public int parsePage(String pagenr)
    {
        try
        {
            return int.Parse(pagenr);
        }
        catch (Exception _)
        {
            return 0;
        }
    }
}
