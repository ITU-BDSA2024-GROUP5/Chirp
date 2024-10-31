using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using Chirp.Infrastructure.Data.DTO;
using SQLitePCL;
using Chirp.Core.DataModels;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    
    [BindProperty]
    [Required]
    [StringLength(160,ErrorMessage = "Maximum length is 160 characters.")]
    public string Text { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;
    private readonly ICheepServiceDB _cheepServiceDb;
    public PublicModel(ICheepRepository cheepRepository,ICheepServiceDB cheepServiceDb)
    {
        _cheepRepository = cheepRepository;
        _cheepServiceDb = cheepServiceDb;
    }

    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "you made an oopsie");
            return Page();
        }

        var author = await _cheepServiceDb.GetAuthorByString(User.Identity.Name);
        
        var cheep = await _cheepServiceDb.CreateCheep(User.Identity?.Name, Text);
        
        
        return RedirectToPage(author);
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
