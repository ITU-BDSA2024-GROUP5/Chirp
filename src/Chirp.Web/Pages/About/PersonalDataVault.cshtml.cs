using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;


namespace Chirp.Web.Pages.About
{
    public class PersonalDataVaultModel(ApplicationDbContext context,
    UserManager<Author> userManager, ICheepRepository cheepRepository) : PageModel
    {
        private readonly ILogger<PersonalDataVaultModel> _logger;

        public string Username { get; private set; }
        public required List<CheepDTO> Cheeps { get; set; }

        public List<PersonalDataItem> PersonalDataItems { get; private set; }

        public async void OnGet(string username)
        {
            Username = username;

            var user = userManager.FindByNameAsync(username).Result;

            await FetchCheeps(username);
            
            PersonalDataItems = new List<PersonalDataItem>
            {
                new PersonalDataItem { Key = "Name", Value = user.UserName},
                new PersonalDataItem { Key = "Email", Value = user.Email },
            };
            if (user.PhoneNumber != null)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "Phone Number", Value = user.PhoneNumber });
            }   
            if (Cheeps.Count > 0)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "Latest Cheep", Value = Cheeps[0].Text });
            }
        
        }

        public async Task FetchCheeps(string author)
        {
        Cheeps = await cheepRepository.ReadByAuthor(0, author);
        }


        public class PersonalDataItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    }
}