using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Core.DataModels;
using Chirp.Infrastructure.Services;

namespace Chirp.Web.Pages;

public class SubmitModel : PageModel
{
        [BindProperty]
        [Required]
        public string Message { get; set; }
        
        public required List<CheepDTO> Cheeps { get; set; }
    
        private readonly ICheepRepository _cheepRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ICheepServiceDB _cheepServiceDB;

        public SubmitModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
        {
                _cheepRepository = cheepRepository;
                _authorRepository = authorRepository;
                _cheepServiceDB = new CheepServiceDB(cheepRepository, authorRepository);
        }

        public async Task<ActionResult> OnGet(string msg)
        {
                Cheep cheep = await _cheepServiceDB.CreateCheep(User.Identity?.Name, msg);
                
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