using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Services;
using Chirp.Infrastructure.Repositories;

namespace Chirp.Web;

//this is the main entry point for the application
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));

        builder.Services.AddAuthentication(options =>
            {
                //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie()
            .AddGitHub(o =>
            {
                o.ClientId = builder.Configuration["authentication_github_clientId"] ?? "defaultClientId";
                o.ClientSecret = builder.Configuration["authentication_github_clientSecret"] ?? "defaultClientSecret";
                o.CallbackPath = "/signin-github";
            });

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();
        builder.Services.AddScoped<ICheepServiceDB, CheepServiceDB>();
        builder.Services.AddScoped<ICheepRepository, CheepRepository>();
        builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddRazorPages();

        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();

        var app = builder.Build();
        
        


        

        // Apply database migrations at startup
        using (var scope = app.Services.CreateScope())
        {
            using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context);
        }
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        app.MapRazorPages();
        //app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSession();
        
        app.MapGet("/debug/env", (IWebHostEnvironment env) => new
        {
            //EnvironmentName = env.EnvironmentName,
            IsProduction = env.IsProduction(),
            IsDevelopment = env.IsDevelopment(),
            AspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
        });
        app.Run();
  }
}