using System.ComponentModel.DataAnnotations;
using Chirp.Core.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

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
    private readonly SignInManager<Author> _signInManager;
    

    public PublicModel(IChirpService chirpService, SignInManager<Author> signInManager)
    {
        _chirpService = chirpService;
        _signInManager = signInManager;
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
            Cheeps = await _chirpService.GetPaginatedResult(CurrentPage, PageSize);
            Count = await _chirpService.GetCount();
            ModelState.AddModelError(string.Empty, "Cheep cannot be empty!");
            return Page();
        }
        
        if (User.Identity == null)
        {
            ModelState.AddModelError(string.Empty, "you must authenticate first");
            return RedirectToPage();
        }
        
        if (Text.Length > 160)
        {
            ModelState.AddModelError(string.Empty, "Cheep is too long");
            return RedirectToPage();
        }
        
        if (Text.Length < 1)
        {
            ModelState.AddModelError(string.Empty, "Cheep is too short");
            return RedirectToPage();
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
    /// Checks whether current user follows another user.
    /// </summary>
    /// <param name="follower"> The person the user would like to follow</param>
    /// <returns></returns>
    public async Task<bool> DoesFollow(string follower)
    {
        var user = await _chirpService.GetAuthorByName(User.Identity!.Name!);
        return user!.Follows.Contains(follower.ToLower());
    }

    /// <summary>
    /// Checks whether there is a user signed in.
    /// </summary>
    /// <returns>True, if _signInManager found a user</returns>
    public bool IsSignedIn()
    {
        return _signInManager.IsSignedIn(User);
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
