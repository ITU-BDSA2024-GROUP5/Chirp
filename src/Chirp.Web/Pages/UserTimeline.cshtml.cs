using System.Collections;
using System.ComponentModel.DataAnnotations;
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services;
using Chirp.Infrastructure.Services.Interfaces;
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
    
    private readonly IChirpService _chirpService;

    public UserTimelineModel(IChirpService chirpService)
    {
        _chirpService = chirpService;
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
        var author = await _chirpService.GetAuthorByName(User.Identity.Name);
        await _chirpService.CreateCheep(author.Name, Text);
        
        await FetchCheeps(author.Name);
        
        return RedirectToPage(author);
    }
    
    public async Task<List<CheepDTO>> FetchCheeps(string author)
    {
        Cheeps = await _chirpService.ReadByAuthor(0, author);
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
        return Page();
    }

    public async Task TaskHandlerAsync(string author)
    {
        AuthorDTO createdAuthor;
        if (author.Contains('@'))
        {
            createdAuthor = await _chirpService.GetAuthorByEmail(author);
        }
        else
        {
            createdAuthor = await _chirpService.GetAuthorByName(author);
        }

        if (createdAuthor == null)
        {
            ModelState.AddModelError(string.Empty, "Author not found");
            return;
        }
        
        if (createdAuthor.Follows.IsNullOrEmpty())
        {
            Cheeps = await _chirpService.ReadByAuthor(GetPage(), createdAuthor.Name);
        }
        else
        {   
            Cheeps = await _chirpService.GetCheepsFollowedByAuthor(GetPage(), createdAuthor.Name, createdAuthor.Follows);
        }
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

        return RedirectToPage();
    }
    
}
