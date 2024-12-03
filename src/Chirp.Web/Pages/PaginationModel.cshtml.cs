using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;

namespace Chirp.Web.Pages;

public class PaginationModel : PageModel
{
    /* 
    private readonly ICheepRepository _cheepRepository;
    
    [BindProperty(SupportsGet = true)]
    public int CurrentPage { get; set; } = 1;
    public int Count { get; set; }
    public int PageSize { get; set; } = 32;
    
    public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
    
    public List<CheepDTO> CheepsPerPage { get; set; }
    
    public async Task OnGetAsync()
    {
        CheepsPerPage = await _cheepRepository.GetPaginatedResultByAuthor(CurrentPage, PageSize);
        Count = await _cheepRepository.GetCount();
    }
    */
}