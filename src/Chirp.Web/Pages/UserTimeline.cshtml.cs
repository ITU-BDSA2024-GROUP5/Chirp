using System.ComponentModel.DataAnnotations;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;

namespace Chirp.Web.Pages;


/// <summary>
/// PageModel for a authors private timeline page.
/// The private timeline page is a subpage where all the cheeps of a specific author are displayed
/// and the cheeps that the author follows.
/// </summary>
public class UserTimelineModel : PageModel 
{
    [BindProperty]
    [Required]
    [StringLength(160,ErrorMessage = "Maximum length is 160 characters.")]
    public string Text { get; set; }
    public required List<CheepDto>? Cheeps { get; set; }
    
    private readonly IChirpService _chirpService;
    public int Count { get; set; }
    public int PageSize { get; set; } = 32;
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    [BindProperty(SupportsGet = true)] public int CurrentPage { get; set; } = 1;

    public UserTimelineModel(IChirpService chirpService)
    {
        _chirpService = chirpService;
        Text = string.Empty;
    }
    
    
    /// <summary>
    /// OnPost method for the private timeline page. This method is called when the user posts a new cheep.
    /// </summary>
    /// <returns></returns>
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

        if (User.Identity != null && User.Identity.Name != null)
        {
            var author = await _chirpService.GetAuthorByName(User.Identity.Name);
            if (author != null)
            {
                await _chirpService.CreateCheep(author.Name, Text);
                await FetchCheeps(author.Name);
            }
            
        }
        return RedirectToPage();
    }
    
    public async Task<List<CheepDto>?> FetchCheeps(string author)
    {
        return Cheeps = await _chirpService.ReadByAuthor(CurrentPage, author);
    }
    
    
    /// <summary>
    /// OnGet method for the private timeline page.
    /// This method is called when the page is loaded to fetch cheeps to display.
    /// </summary>
    /// <param name="author">The Author to fecth cheeps by</param>
    /// <returns>The private page</returns>
    public async Task<ActionResult> OnGet(string author)
    {
        if (!string.IsNullOrEmpty(author))
        {
            await TaskHandlerAsync(author);
        }
        
        return Page();
    }

    
    /// <summary>
    /// Task handler for the private timeline page.
    /// Handles searching for an author by name or email.
    /// Also handles fetching all cheeps by authors followed by the author.
    /// Only fetches the authors cheeps if the logged in author is not the same as the author searched for.
    /// </summary>
    /// <param name="author"></param>
    public async Task TaskHandlerAsync(string author)
    {
        if (author.Equals("Oauth.styles.css")) return;
        
        AuthorDto? createdAuthor;
        if (author.Contains('@'))
        {
            createdAuthor = await _chirpService.GetAuthorByEmail(author);
            
            if (createdAuthor == null)
            {
                ModelState.AddModelError(string.Empty, "Author not found");
                return;
            }
            
            Cheeps = await _chirpService.ReadByAuthor(CurrentPage, createdAuthor.Name);

            if (User.Identity?.Name == createdAuthor.Name)
            {
                Cheeps = await _chirpService.GetCheepsFollowedByAuthor(CurrentPage, createdAuthor.Name, createdAuthor.Follows);
                Count = await _chirpService.GetCheepsCountByFollows(author, createdAuthor.Follows);
            }
            return;
        }
        
        createdAuthor = await _chirpService.GetAuthorByName(author);

        if (createdAuthor == null)
        {
            ModelState.AddModelError(string.Empty, "Author not found");
            return;
        }
        
        if (createdAuthor.Follows.IsNullOrEmpty())
        {
            Cheeps = await _chirpService.ReadByAuthor(CurrentPage, createdAuthor.Name);
            return;
        }
        
        if (User.Identity != null && User.Identity.Name == createdAuthor.Name)
        {   
            Cheeps = await _chirpService.GetCheepsFollowedByAuthor(CurrentPage, createdAuthor.Name, createdAuthor.Follows);
            Count = await _chirpService.GetCheepsCountByFollows(author, createdAuthor.Follows);
            return;
        }
        
        Cheeps = await _chirpService.GetPaginatedResultByAuthor(CurrentPage, createdAuthor.Name, PageSize);
        var cheepDtos = _chirpService.GetCheepsByAuthor(createdAuthor.Name).Result;
        if (cheepDtos != null)
            Count = cheepDtos.Count;
    }
    
    
    /// <summary>
    /// Gets the page number from the query string.
    /// </summary>
    /// <returns></returns>
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
    
    
    /// <summary>
    /// OnPost method for the public page. This method is handles displaying the proper text for the follow button.
    /// Depending on if the logged-in user is following the author or not, the text will change.
    /// </summary>
    /// <param name="authorToFollow">The author to follow or un-follow</param>
    /// <returns>Page reload</returns>
    public async Task<IActionResult> OnPostToggleFollow(string authorToFollow)
    {
        if (User.Identity != null && User.Identity.Name != null)
        {
            var author = await _chirpService.GetAuthorByName(User.Identity.Name);
        
            var isFollowing = await _chirpService.ContainsFollower(authorToFollow, User.Identity.Name);

            if (isFollowing && author != null)
            {
                await _chirpService.RemoveFollows(author.Name, authorToFollow);
            }
            else if (author != null)
            {
                await _chirpService.AddFollows(author.Name, authorToFollow);
            }
        }

        return RedirectToPage();
    }
    
}
