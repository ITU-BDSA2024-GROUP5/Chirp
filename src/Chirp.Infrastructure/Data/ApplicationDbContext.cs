using Chirp.Core.DataModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chirp.Infrastructure.Data;


/// <summary>
/// The DbContext for the Chirp application. Contains the Cheeps and Authors tables.
/// Also contains all tables for the ASP.NET Identity system. (AspNetUsers...)
/// </summary>
/// <param name="options"></param>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<Author>(options)
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

}
