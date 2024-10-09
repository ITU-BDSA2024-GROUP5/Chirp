using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    public List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;

    public PublicModel(ICheepRepository cheepRepository)
    {
        _cheepRepository = cheepRepository;
    }

    public async Task<ActionResult> OnGet()
    {
        //var authors = from a in _cheepRepository.ReadByAuthor();
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

    public IList<Author> Authors { get; set; } = default!;
    
    [BindProperty(SupportsGet = true)]
    public string? SearchString { get; set; }
    
    public SelectList? Name { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Email { get; set; }
    
}
