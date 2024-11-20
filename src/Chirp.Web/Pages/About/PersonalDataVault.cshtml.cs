using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace Chirp.Web.Pages.About
{
    public class PersonalDataVaultModel : PageModel
    {
        private readonly ILogger<PersonalDataVaultModel> _logger;

        public string Username { get; private set; }

        public PersonalDataVaultModel(ILogger<PersonalDataVaultModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string username)
        {
            Username = username;
        }
    }
}