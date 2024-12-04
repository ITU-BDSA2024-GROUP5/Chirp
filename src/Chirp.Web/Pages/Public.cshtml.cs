using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;

namespace Chirp.Web.Pages;


/// <summary>
/// PageModel for the public page. The public page is the main page where cheeps are displayed.
/// </summary>
public class PublicModel : PageModel
{
     
    [BindProperty]
    [Required]
    [StringLength(160, MinimumLength = 1, ErrorMessage = "String length must be between 1 and 160")]
    public string Text { get; set; }
    public required List<CheepDto>? Cheeps { get; set; }
    
    private readonly IChirpService _chirpService;
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int Count { get; set; }
    public int PageSize { get; set; } = 32;
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    public PublicModel(IChirpService chirpService)
    {
        _chirpService = chirpService;
        Text = string.Empty;
    }
    
    
    /// <summary>
    /// OnPost method for the public page. This method is called when the user posts a new cheep.
    /// </summary>
    /// <returns>Page reload</returns>
    public async Task<ActionResult> OnPost()
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "you made an oopsie");
            return Page();
        }
        
        var author = await _chirpService.GetAuthorByName(User.Identity.Name);
        await _chirpService.CreateCheep(author.Name, Text);
        
        await FetchCheeps(author.Name);
        
        return RedirectToPage(author);
    }
    
    
    private async Task FetchCheeps(string author)
    {
        Cheeps = await _chirpService.ReadByAuthor(0, author);
    }
    
    public async Task FetchCheeps()
    {
        Cheeps = await _chirpService.Read(0);
    }
    
    
    /// <summary>
    /// OnPost method for the public page. This method is handles displaying the proper text for the follow button.
    /// Depending on if the logged-in user is following the author or not, the text will change.
    /// </summary>
    /// <param name="authorToFollow">The author to follow or un-follow</param>
    /// <returns>Page reload</returns>
    public async Task<IActionResult> OnPostToggleFollow(string authorToFollow)
    {
        var author = await _chirpService.GetAuthorByName(User.Identity.Name);
        
        var IsFollowing = await _chirpService.ContainsFollower(authorToFollow, User.Identity.Name);

        if (IsFollowing)
        {
            await _chirpService.RemoveFollows(author.Name, authorToFollow);
        }
        else
        {
            await _chirpService.AddFollows(author.Name, authorToFollow);
        }
        
        IsFollowing = !IsFollowing;
        
        return RedirectToPage();
    }
    
    
    /// <summary>
    /// OnGet method for the public page.
    /// This method is called when the page is loaded to fetch cheeps to display on the public page.
    /// </summary>
    /// <returns>The public page</returns>
    public async Task<ActionResult> OnGet()
    {
        Cheeps = await _chirpService.GetPaginatedResult(CurrentPage, PageSize);
        Count = await _chirpService.GetCount();
        return Page();
    }

    
    /// <summary>
    /// Parses the page number from the query string from string to integer.
    /// </summary>
    /// <param name="pagenr">Page number string to parse</param>
    /// <returns>Integer</returns>
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
