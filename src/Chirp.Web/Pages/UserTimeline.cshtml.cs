using System.ComponentModel.DataAnnotations;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel 
{
    [BindProperty]
    [Required]
    [StringLength(160,ErrorMessage = "Maximum length is 160 characters.")]
    public string Text { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepServiceDB _cheepServiceDb;

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
        _cheepServiceDb = new CheepServiceDB(cheepRepository, authorRepository);
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
    
    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(author))
        {
            await taskHandlerAsync(author);
        }
        return Page();
    }

    public async Task taskHandlerAsync(string author)
    {
        
        AuthorDTO createdAuthor;
        var isEmail = false;
        
        if (author.Contains('@'))
        {
            isEmail = true;
            createdAuthor = await _authorRepository.GetAuthorByEmail(author);
        }
        else
        {
            createdAuthor = await _authorRepository.GetAuthorByName(author);
        }
    
        await fetchCheeps(author, isEmail);
        
    }

    public async Task fetchCheeps(string author, bool isEmail)
    {
        if (isEmail)
        {
            Cheeps = await _cheepRepository.ReadByEmail(getPage(), author);
        }
        else
        {
            Cheeps = await _cheepRepository.ReadByAuthor(getPage(), author);
        }
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
