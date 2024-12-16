using System.ComponentModel.DataAnnotations;
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
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
    private readonly SignInManager<Author> _signInManager;


    public UserTimelineModel(IChirpService chirpService, SignInManager<Author> signInManager)
    {
        _chirpService = chirpService;
        _signInManager = signInManager;
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
    
    public async Task FetchCheeps(string author)
    {
        Cheeps = await _chirpService.ReadByAuthor(CurrentPage, author);
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
        
        // var tmpAuthor = await _chirpService.GetAuthorByName(author);
        // if (User.Identity != null && User.Identity.Name == author)
        // {
        //     Cheeps = await _chirpService.GetCheepsFollowedByAuthor(CurrentPage, author, tmpAuthor?.Follows);
        //     Count = await _chirpService.GetCheepsCountByFollows(author, tmpAuthor?.Follows);
        //     return Page();
        // }
        //
        // Cheeps = await _chirpService.GetPaginatedResultByAuthor(CurrentPage, author, PageSize);
        // var cheepDtos = _chirpService.GetCheepsByAuthor(author).Result;
        // if (cheepDtos != null)
        //     Count = cheepDtos.Count;
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
        
        if (author.Contains('@'))
        {
            await SearchByEmail(author);
            return;
        }

        await SearchByName(author);
    }

    /// <summary>
    /// Handles fetching cheeps by author mail.
    /// </summary>
    /// <param name="author">Author email to fetch cheeps by.</param>
    private async Task SearchByEmail(string author)
    {
        AuthorDto? createdAuthor = await _chirpService.GetAuthorByEmail(author);

        if (createdAuthor == null)
        {
            ModelState.AddModelError(string.Empty, "Author not found");
            return;
        }
        
        // if (createdAuthor.Follows.IsNullOrEmpty())
        // {
        //     Cheeps = await _chirpService.ReadByAuthor(CurrentPage, createdAuthor.Name);
        //     return;
        // }
        
        if (User.Identity != null && User.Identity.Name == createdAuthor.Name)
        {   
            Cheeps = await _chirpService.GetCheepsFollowedByAuthor(CurrentPage, createdAuthor.Name, createdAuthor.Follows);
            if (createdAuthor.Follows.IsNullOrEmpty())
            {
                var cheepDtos = _chirpService.ReadAllCheeps(createdAuthor.Name).Result;
                if (cheepDtos != null)
                    Count = cheepDtos.Count;
            }
            else
            {
                Count = await _chirpService.GetCheepsCountByFollows(author, createdAuthor.Follows);

            }
        }
        else
        {
            Cheeps = await _chirpService.ReadByAuthor(CurrentPage, createdAuthor.Name);
            if (Cheeps != null) Count = Cheeps.Count;
        }
    }
    
    /// <summary>
    /// Handles fetching cheeps by author name.
    /// </summary>
    /// <param name="author">Author name to et cheeps by.</param>
    private async Task SearchByName(string author)
    {
        AuthorDto? createdAuthor = await _chirpService.GetAuthorByName(author);

        if (createdAuthor == null)
        {
            ModelState.AddModelError(string.Empty, "Author not found");
            return;
        }
        
        // if (createdAuthor.Follows.IsNullOrEmpty())
        // {
        //     Cheeps = await _chirpService.ReadByAuthor(CurrentPage, createdAuthor.Name);
        //     return;
        // }
        
        if (User.Identity != null && User.Identity.Name == createdAuthor.Name)
        {   
            Cheeps = await _chirpService.GetCheepsFollowedByAuthor(CurrentPage, createdAuthor.Name, createdAuthor.Follows);
            if (createdAuthor.Follows.IsNullOrEmpty())
            {
                var cheepDtos = _chirpService.ReadAllCheeps(createdAuthor.Name).Result;
                if (cheepDtos != null)
                    Count = cheepDtos.Count;
            }
            else
            {
                Count = await _chirpService.GetCheepsCountByFollows(author, createdAuthor.Follows);

            }
        }
        else
        {
            Cheeps = await _chirpService.ReadByAuthor(CurrentPage, createdAuthor.Name);
            var cheepDtos = _chirpService.ReadAllCheeps(createdAuthor.Name).Result;
            if (cheepDtos != null)
                Count = cheepDtos.Count;
        }
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
    /// Checks whether current user follows another user.
    /// </summary>
    /// <param name="follower"> The person the user would like to follow</param>
    /// <returns></returns>
    public async Task<bool> DoesFollow(string follower)
    {
        var user = await _chirpService.GetAuthorByName(User.Identity!.Name!);
        return await _chirpService.ContainsFollower(follower.ToLower(), user!.Name);
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
