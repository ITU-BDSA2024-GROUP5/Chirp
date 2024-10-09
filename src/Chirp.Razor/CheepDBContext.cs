using Microsoft.EntityFrameworkCore;

public class CheepDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public CheepDBContext(DbContextOptions<CheepDBContext> options) : base(options)
    {
    }
}