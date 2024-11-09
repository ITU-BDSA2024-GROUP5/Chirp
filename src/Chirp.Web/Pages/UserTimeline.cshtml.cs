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
        Text = string.Empty;
    }
    
    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "you made an oopsie");
            return Page();
        }

        if (User.Identity == null)
        {
            ModelState.AddModelError(string.Empty, "you must authenticate first");
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
    
    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(author))
        {
            await TaskHandlerAsync(author);
        }
        return Page();
    }

    public async Task TaskHandlerAsync(string author)
    {
        
        AuthorDTO createdAuthor;
        if (author.Contains('@'))
        {
            createdAuthor = await _authorRepository.GetAuthorByEmail(author);
        }
        else
        {
            createdAuthor = await _authorRepository.GetAuthorByName(author);
        }

        if (createdAuthor == null)
        {
            ModelState.AddModelError(string.Empty, "Author not found");
            return;
        }
        author = createdAuthor.Name;
        
        Cheeps = await _cheepRepository.ReadByAuthor(GetPage(), author);
        
    }
    
    public int GetPage()
    {
        try
        {
            return int.Parse(Request.Query["page"].ToString());
        }
        catch (Exception)
        {
            return 0;
        }
    }
}
