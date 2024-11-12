using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Admin;

public class AdminModel(
    ApplicationDbContext context,
    UserManager<Author> userManager,
    IWebHostEnvironment env)
    : PageModel
{
    public ICollection<Author> Users { get; set; }
    
    public void OnGet()
    {
        Users = context.Authors.ToList();
    }
    
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext execcontext)
    {
        // ASPNETCORE_ENVIRONMENT is not set to "Development" in Properties/launchSettings.json: return 404.
        if (!env.IsDevelopment())  // using Microsoft.Extensions.Hosting;
        {
            execcontext.Result = NotFound();
        }
    }

    public async Task<IActionResult> OnPostResetDatabase()
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedDatabase(context, userManager);
        return RedirectToPage();
    }
}

