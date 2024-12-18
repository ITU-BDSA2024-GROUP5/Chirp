using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Chirp.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Build.Framework;

namespace Chirp.Web.Pages.Admin;


/// <summary>
/// PageModel for the admin panel.
/// The admin panel is a page where the admin can reset the database and see info about all users and delete them.
/// </summary>
/// <param name="context">ApplicationDbContext that contains Cheeps and Authors</param>
/// <param name="userManager"></param>
/// <param name="env"></param>
public class AdminModel(
    ApplicationDbContext context,
    UserManager<Author> userManager,
    IWebHostEnvironment env)
    : PageModel
{
    public required ICollection<Author> Users { get; set; }
    
    
    
    /// <summary>
    /// Sets the Users property to all Authors in the database.
    /// </summary>
    public void OnGet()
    {
        Users = context.Authors.ToList();
    }
    
    
    /// <summary>
    /// Restricts the admin panel to development environment only.
    /// </summary>
    /// <param name="execcontext"></param>
    public override void OnPageHandlerExecuting(PageHandlerExecutingContext execcontext)
    {
        // ASPNETCORE_ENVIRONMENT is not set to "Development" in Properties/launchSettings.json: return 404.
        if (!env.IsDevelopment())  // using Microsoft.Extensions.Hosting;
        {
            execcontext.Result = NotFound();
        }
    }

    
    /// <summary>
    /// Resets the database by deleting all data and reseeding it.
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResult> OnPostResetDatabase()
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedDatabase(context, userManager);
        return RedirectToPage();
    }
    
    
    [BindProperty]
    [Required]
    public required string? SelectedUserId { get; set; }
    
    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <returns></returns>
    public async Task<ActionResult> OnPostDeleteUser()
    {
        if (SelectedUserId != null)
        {
            var user = await userManager.FindByIdAsync(SelectedUserId);
            if (user != null)
            { 
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    var authorCheeps = context.Cheeps.ToList().Where(c => c.AuthorId.ToString().Equals(user.Id));
                    context.Authors.Remove(user);
                    context.Cheeps.RemoveRange(authorCheeps);
                }
            }
        }

        return RedirectToPage();
    }
}

