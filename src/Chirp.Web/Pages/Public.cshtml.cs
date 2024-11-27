using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services.Interfaces;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
     
    [BindProperty]
    [Required]
    [StringLength(160, MinimumLength = 1, ErrorMessage = "String length must be between 1 and 160")]
    public string Text { get; set; }
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly IChirpService _chirpService;
    public PublicModel(IChirpService chirpService)
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
        
        var author = await _chirpService.GetAuthorByName(User.Identity.Name);
        await _chirpService.CreateCheep(author.Name, Text);
        
        await FetchCheeps(author.Name);
        
        return RedirectToPage(author);
    }
    
    public async Task FetchCheeps(string author)
    {
        Cheeps = await _chirpService.ReadByAuthor(0, author);
    }
    
    public async Task FetchCheeps()
    {
        Cheeps = await _chirpService.Read(0);
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
        
        IsFollowing = !IsFollowing;
        
        return RedirectToPage();
    }
    
    public async Task<ActionResult> OnGet()
    {
        Cheeps = await _chirpService.Read(ParsePage(Request.Query["page"].ToString()));
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
