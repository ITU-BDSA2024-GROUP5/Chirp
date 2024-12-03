using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System.Collections.Generic;
using Chirp.Infrastructure.Data.DTO;
using System.Text;
using Chirp.Infrastructure.Services.Interfaces;


namespace Chirp.Web.Pages.About
{
    public class PersonalDataVaultModel(ApplicationDbContext context,
    UserManager<Author> userManager, IChirpService chirpService) : PageModel
    {
        //private readonly ILogger<PersonalDataVaultModel> _logger;

        public string? Username { get; private set; }

        public string CheepButtonText { get; set; } = "Show Cheeps";

        public string CheepButtonFunction { get; set; } = "ShowCheeps";
        public string FollowerButtonText { get; set; } = "Show Followed";
        public string FollowerButtonFunction { get; set; } = "ShowFollowed";
        public static List<CheepDTO>? Cheeps { get; private set; }

        public static List<string> Followed { get; private set; }

        public static Author? user { get; private set; }

        public List<PersonalDataItem>? PersonalDataItems { get; private set; }


        /// <summary>
        /// Constructor for the PersonalDataVaultModel
        /// </summary>
        /// <param name="username"></param>
        public async void OnGet(string username)
        {
            Username = username;

            user = userManager.FindByNameAsync(username).Result;

            await FetchCheeps(Username);
            PersonalDataItems = new List<PersonalDataItem>();

            if (user == null) return;
            
            if (user.UserName != null )
            {
                PersonalDataItems.Add(new PersonalDataItem("Name",user.UserName));
            }
            if (user.Email != null)
            {
                PersonalDataItems.Add(new PersonalDataItem("Email",user.Email));
            }
            if (user.PhoneNumber != null)
            {
                PersonalDataItems.Add(new PersonalDataItem("Phone Number",user.PhoneNumber));
            }   
            if (Cheeps != null)
            {
                PersonalDataItems.Add(new PersonalDataItem("Latest Cheep",Cheeps[0].Text));
            }
        }

        /// <summary>
        /// Function for showing all the cheeps of the user
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostShowCheeps()
        {
            PersonalDataItems = new List<PersonalDataItem>();
            if (user == null) return Page();
            if (Cheeps == null) return Page();
            Username = user.UserName;
            if(Cheeps.Count == 0)
            {
                PersonalDataItems.Add(new PersonalDataItem("No cheeps to show"," "));
                return Page();
            }

            
            foreach (var cheep in Cheeps)
            {
                PersonalDataItems.Add(new PersonalDataItem(cheep.TimeStamp,cheep.Text));
            }
            
            CheepButtonText = "Go back";

            return Page();
        }

        /// <summary>
        /// Function for showing all the people the user follows
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostShowFollowed()
        {
            PersonalDataItems = new List<PersonalDataItem>();
            Username = user.UserName;
            if(Followed.Count == 0)
            {
                PersonalDataItems.Add(new PersonalDataItem("No followed people to show",""));
                FollowerButtonFunction = "Go back";

                return Page();
            }
            foreach (var follower in Followed)
            {
                PersonalDataItems.Add(new PersonalDataItem(follower,""));
            }
            
            FollowerButtonText = "Go back";
            

            return Page();
        }

        /// <summary>
        /// Function for going back to the previous page / main page of the personaldatavault
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Function allowing the user to download their data in a csv file
        /// </summary>
        /// <returns></returns>
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

            
            if(Cheeps.Count > 0)
            {
                foreach (var cheep in Cheeps)
                {
                    csv.AppendLine($"{cheep.TimeStamp},{cheep.Text}");
                }
            }

            if (Cheeps != null)
            {
                foreach (var cheep in Cheeps)
                {
                    csv.AppendLine($"{cheep.TimeStamp},{cheep.Text}");
                }
            }


            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{user.UserName}_data.csv");
        }

        /// <summary>
        /// Function for redirecting the user to the delete personal data page
        /// Relies on the Identity framework
        /// Redirects to /Identity/Account/Manage/DeletePersonalData
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> OnPostDeleteData()
        {
            return Redirect($"/Identity/Account/Manage/DeletePersonalData");
        }

        /// <summary>
        /// Function for fetching all the cheeps of the user
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        public async Task FetchCheeps(string author)
        {
            Cheeps = await chirpService.ReadAllCheeps(author);
        }

        /// <summary>
        /// Function for fetching all the people the user follows
        /// </summary>
        /// <param name="author"></param>
        /// <returns></returns>
        public async Task<List<string>>? FetchFollowed(string author)
        {
            Followed = await chirpService.GetFollowed(author);
            return Followed;
        }
        
        /// <summary>
        /// Class for the personal data items that are shown on the page
        /// Contains a key and a value for each item displayed in the table on the page
        /// </summary>
        public class PersonalDataItem
        {
            public PersonalDataItem(string key, string value)
            {
                Key = key;
                Value = value; 
            }
            
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}