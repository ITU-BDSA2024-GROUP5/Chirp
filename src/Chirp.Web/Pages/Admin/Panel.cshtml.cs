using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

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
    
    
    [BindProperty]
    [Required]
    public string selectedUserID { get; set; }
    public async Task<ActionResult> OnPostDeleteUser()
    {
        Console.WriteLine("hallo "+selectedUserID);
        var user = await userManager.FindByIdAsync(selectedUserID);
        if (user != null)
        { 
            var result = await userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                var authorCheeps = context.Cheeps.ToList().Where(c => c.AuthorId.ToString().Equals(user.Id));
                context.Cheeps.RemoveRange(authorCheeps);
            }
        }
        return RedirectToPage();
    }
}

