using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;
using System.Collections.Generic;

namespace Chirp.Web.Pages.About
{
    public class PersonalDataVaultModel(ApplicationDbContext context,
    UserManager<Author> userManager) : PageModel
    {
        private readonly ILogger<PersonalDataVaultModel> _logger;

        public string Username { get; private set; }

        public List<PersonalDataItem> PersonalDataItems { get; private set; }

        public void OnGet(string username)
        {
            Username = username;
            var user = userManager.FindByNameAsync(username).Result;
            
            PersonalDataItems = new List<PersonalDataItem>
            {
                new PersonalDataItem { Key = "Email", Value = user.Email },
            };
        }
        public class PersonalDataItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    }
}