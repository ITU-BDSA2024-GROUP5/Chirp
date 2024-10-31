using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SQLitePCL;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel 
{
    public required List<CheepDTO> Cheeps { get; set; }
    
    private readonly ICheepRepository _cheepRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepServiceDB _cheepServiceDB;

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
        _cheepRepository = cheepRepository;
        _cheepServiceDB = new CheepServiceDB(cheepRepository, authorRepository);
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
