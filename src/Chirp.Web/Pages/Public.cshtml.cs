using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
     
    [BindProperty]
    [Required]
    [StringLength(160, MinimumLength = 1, ErrorMessage = "String length must be between 1 and 160")]
    public string Text { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;
    private readonly ICheepServiceDB _cheepServiceDb;
    private readonly IAuthorRepository _authorRepository;
    public PublicModel(ICheepRepository cheepRepository, ICheepServiceDB cheepServiceDb, AuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _cheepServiceDb = cheepServiceDb;
        _authorRepository = authorRepository;
        Text = string.Empty;
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
        
        await FetchCheeps(author.Name);
        
        return RedirectToPage(author);
    }
    
    public async Task FetchCheeps(string author)
    {
            Cheeps = await _cheepRepository.ReadByAuthor(0, author);
    }
    
    public async Task FetchCheeps()
    {
        Cheeps = await _cheepRepository.Read(0);
    }

    public async Task FetchAuthorFollows()
    {
        
    }
    
    public async Task<ActionResult> OnGet()
    {
        Cheeps = await _cheepRepository.Read(ParsePage(Request.Query["page"].ToString()));
        return Page();
    }

    public int ParsePage(string pagenr)
    {
        try
        {
            return int.Parse(pagenr);
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
