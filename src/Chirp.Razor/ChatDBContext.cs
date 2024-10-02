using Microsoft.EntityFrameworkCore;

public class ChatDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=Chat.db"); // If not overrided, we get error message: Table __EFMigrationsHistory doesn't exist
        }
    }
}