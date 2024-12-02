using Chirp.Core.DataModels;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Data.DTO;
using System.Text;
using Chirp.Infrastructure.Services.Interfaces;


namespace Chirp.Web.Pages.About
{
    public class PersonalDataVaultModel(ApplicationDbContext context,
    UserManager<Author> userManager, IChirpService chirpService) : PageModel
    {
        private readonly ILogger<PersonalDataVaultModel> _logger;

        public string Username { get; private set; }

        public string CheepButtonText { get; set; } = "Show Cheeps";

        public string CheepButtonFunction { get; set; } = "ShowCheeps";

        public string FollowerButtonText { get; set; } = "Show Followed";

        public string FollowerButtonFunction { get; set; } = "ShowFollowed";
        public static List<CheepDTO> Cheeps { get; private set; }

        public static List<string> Followed { get; private set; }

        public static Author? user { get; private set; }

        public List<PersonalDataItem> PersonalDataItems { get; private set; }

        public async void OnGet(string username)
        {
            Username = username;

            user = userManager.FindByNameAsync(username).Result;

            await FetchCheeps(Username);
            await FetchFollowed(Username);
            
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

            if(Followed.Count > 0)
            {
                 PersonalDataItems.Add(new PersonalDataItem { Key = "Latest followed", Value = string.Join(", ", Followed) });
            }
            

            
        }

        public async Task<IActionResult> OnPostShowCheeps()
        {
            PersonalDataItems = new List<PersonalDataItem>();
            Username = user.UserName;
            if(Cheeps.Count == 0)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "No cheeps to show", Value = "" });
                CheepButtonText = "Go back";
                CheepButtonFunction = "GoBack";

                return Page();
            }
            foreach (var cheep in Cheeps)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = cheep.TimeStamp, Value = cheep.Text });
            }
            
            CheepButtonText = "Go back";
            CheepButtonFunction = "GoBack";

            return Page();
        }

        public async Task<IActionResult> OnPostShowFollowed()
        {
            PersonalDataItems = new List<PersonalDataItem>();
            Username = user.UserName;
            if(Followed.Count == 0)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = "No followed people to show", Value = "" });
                FollowerButtonText = "Go back";
                FollowerButtonFunction = "GoBack";

                return Page();
            }
            foreach (var follower in Followed)
            {
                PersonalDataItems.Add(new PersonalDataItem { Key = follower, Value = "" });
            }
            
            FollowerButtonText = "Go back";
            FollowerButtonFunction = "GoBack";

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
            if(Cheeps.Count > 0)
            {
                foreach (var cheep in Cheeps)
                {
                    csv.AppendLine($"{cheep.TimeStamp},{cheep.Text}");
                }
            }

            if(Followed.Count > 0)
            {
                foreach (var follower in Followed)
                {
                    csv.AppendLine($"Follow,{follower}");
                }
            }


            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"{user.UserName}_data.csv");
        }


        public async Task<IActionResult> OnPostDeleteData()
        {
            return Redirect($"/Identity/Account/Manage/DeletePersonalData");
        }

        public async Task FetchCheeps(string author)
        {
            Cheeps = await chirpService.ReadAllCheeps(author);
        }

        public async Task FetchFollowed(string author)
        {
            Followed = await chirpService.GetFollowed(author);
        }
        

        public class PersonalDataItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}