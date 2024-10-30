using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Oauth.Data;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication.Cookies;


public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));


        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddRazorPages();

        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();

        var app = builder.Build();


        
       

        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.MapRazorPages();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();

        app.Run();
  }
}