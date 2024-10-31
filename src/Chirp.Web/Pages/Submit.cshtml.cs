using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Services;

namespace Chirp.Web.Pages;

public class SubmitModel : PageModel
{
        [BindProperty]
        [Required]
        public string Message { get; set; }
        
        public required List<CheepDTO> Cheeps { get; set; }
    
        private readonly ICheepRepository _cheepRepository;
        private readonly ICheepServiceDB _cheepServiceDB;

        public SubmitModel(ICheepRepository cheepRepository)
        {
                _cheepRepository = cheepRepository;
                _cheepServiceDB = new CheepServiceDB(cheepRepository);
        }

        public async Task<ActionResult> OnGet(string msg)
        {
                var author = await _cheepRepository.GetAuthorByName(User.Identity?.Name);
                Cheep cheep = await _cheepServiceDB.CreateCheep(author, msg);

                Cheeps.Add(new CheepDTO
                {
                        Text = cheep.Text,
                        Author = cheep.Author.Name,
                        TimeStamp = cheep.TimeStamp.ToString()
                });
                Message = msg;
                return Page();
        }
}