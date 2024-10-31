using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.AspNetCore.Mvc.Rendering;
using SQLitePCL;

namespace Oauth.Pages;

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
        if (author!=null)
        {
            var cheep = await _cheepServiceDb.CreateCheep(author, Text);
            _cheepRepository.WriteCheep(cheep);
            _cheepServiceDb.WriteCheep(cheep);
        }
        else
        {
            Author newAuthor = new Author()
            {
                Name = User.Identity.Name,
                AuthorId = await _cheepRepository.GetHighestAuthorId() + 1,
                Email = User.Identity.Name,
                Cheeps = new List<Cheep>()
            };
            author = newAuthor;
            var cheep = await _cheepServiceDb.CreateCheep(newAuthor, Text);
            _cheepRepository.WriteCheep(cheep);
            _cheepServiceDb.WriteCheep(cheep);
        }
        await fetchCheeps(author.ToString());
        
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
