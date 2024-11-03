using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    
    [BindProperty]
    [Required]
    [StringLength(160,ErrorMessage = "Maximum length is 160 characters.")]
    public string Text { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepServiceDB _cheepServiceDb;
    public PublicModel(ICheepRepository cheepRepository, ICheepServiceDB cheepServiceDb, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
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
        if (author == null)
        {
            var newAuthor = await _cheepServiceDb.CreateAuthor(User.Identity.Name);
            author = newAuthor;
        }
        var cheep = await _cheepServiceDb.CreateCheep(author.Name, Text);
        await _cheepServiceDb.WriteCheep(cheep);
        
        await fetchCheeps(author.Name);
        
        return RedirectToPage(author);
    }
    
    public async Task fetchCheeps(string author)
    {
            Cheeps = await _cheepRepository.ReadByAuthor(0, author);
    }
    

    public async Task<ActionResult> OnGet()
    {
        Cheeps = await _cheepRepository.Read(parsePage(Request.Query["page"].ToString()));
        return Page();
    }

    public int parsePage(string pagenr)
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
