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
using System.Text;


namespace Chirp.Web.Pages.About
{
    public class PersonalDataVaultModel(ApplicationDbContext context,
    UserManager<Author> userManager, ICheepRepository cheepRepository) : PageModel
    {
        private readonly ILogger<PersonalDataVaultModel> _logger;

        public string Username { get; private set; }

        public string ButtonText { get; set; } = "Show Cheeps";

        public string ButtonFunction { get; set; } = "ShowCheeps";
        public static List<CheepDTO> Cheeps { get; private set; }

        public static Author? user { get; private set; }

        public List<PersonalDataItem> PersonalDataItems { get; private set; }

        public async void OnGet(string username)
        {
            Username = username;

            user = userManager.FindByNameAsync(username).Result;

            await FetchCheeps(Username);
            
            PersonalDataItems = new List<PersonalDataItem>
            {
                new PersonalDataItem { Key = "Name", Value = user.UserName},
                new PersonalDataItem { Key = "Email", Value = user.Email },
            };
            if (user.PhoneNumber != null)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "Phone Number", Value = user.PhoneNumber });
            }   
            if (!IsCheepEmpty())
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "Latest Cheep", Value = Cheeps[0].Text });
            }
            
        }

        public async Task<IActionResult> OnPostShowCheeps()
        {
            PersonalDataItems = new List<PersonalDataItem>();
            Username = user.UserName;
            if(IsCheepEmpty())
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "No cheeps to show", Value = "" });
                ButtonText = "Go back";
                ButtonFunction = "GoBack";

                return Page();
            }
            foreach (var cheep in Cheeps)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = cheep.TimeStamp, Value = cheep.Text });
            }
            
            ButtonText = "Go back";
            ButtonFunction = "GoBack";

            return Page();
        }

        public async Task<IActionResult> OnPostGoBack()
        {
           var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                // Handle the case where the user is not authenticated
                return RedirectToPage("/Index");
            }

            return Redirect($"/About/PersonalDataVault/{username}"); 
        }

        //Function for downloading data as a CSV file
        public async Task<IActionResult> OnPostDownloadData()
        {
            var csv = new StringBuilder();
            csv.AppendLine("Timestamp,Text");
            

            csv.AppendLine($"Name,{user.UserName}");
            csv.AppendLine($"Email,{user.Email}");
            if (user.PhoneNumber != null)
            {
                csv.AppendLine($"Phone Number,{user.PhoneNumber}");
            }
            if(IsCheepEmpty())
            {
                return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{user.UserName}_data.csv");
            }

            foreach (var cheep in Cheeps)
            {
                csv.AppendLine($"{cheep.TimeStamp},{cheep.Text}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{user.UserName}_data.csv");
        }

        public async Task<IActionResult> OnPostDeleteData()
        {
            return Redirect($"/Identity/Account/Manage/DeletePersonalData");
        }

        public async Task FetchCheeps(string author)
        {
            Cheeps = await cheepRepository.ReadAllCheeps(author);
        }

        public Boolean IsCheepEmpty()
        {
            return Cheeps.Count == 0;
        }

        public class PersonalDataItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}