using System.Collections;
using System.ComponentModel.DataAnnotations;
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

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
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int Count { get; set; }
    public int PageSize { get; set; } = 32;
    
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    
    public List<CheepDTO> CheepsPerPage { get; set; }

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
    
    public async Task<List<CheepDTO>> FetchCheeps(string author)
    {
        Cheeps = await _cheepRepository.ReadByAuthor(0, author);
        Cheeps = Cheeps
            .OrderBy(c => DateTime.Parse(c.TimeStamp).Date) // Parse and sort by DateTime
            .ToList();
        return Cheeps;
    }
    
    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(author))
        {
            await TaskHandlerAsync(author);
        }

        Cheeps = await _cheepRepository.GetPaginatedResultByAuthor(CurrentPage, author, PageSize);
        Count = (await _cheepRepository.GetCheepsByAuthor(author)).Count;
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
        
        if (createdAuthor.Follows.IsNullOrEmpty())
        {
            Cheeps = await _cheepRepository.ReadByAuthor(CurrentPage, createdAuthor.Name);
        }
        else
        {   
            Cheeps = await _cheepRepository.GetCheepsFollowedByAuthor(CurrentPage, createdAuthor.Name, createdAuthor.Follows);
        }
    }
    
    public async Task<IActionResult> OnPostToggleFollow(string authorToFollow)
    {
        Author author = await _authorRepository.GetAuthorByNameEntity(User.Identity.Name);
        
        var IsFollowing = await _authorRepository.ContainsFollower(authorToFollow, User.Identity.Name);

        if (IsFollowing)
        {
            await _authorRepository.RemoveFollows(author.UserName, authorToFollow);
        }
        else
        {
            await _authorRepository.AddFollows(author.UserName, authorToFollow);
        }

        return RedirectToPage();
    }
    
}
