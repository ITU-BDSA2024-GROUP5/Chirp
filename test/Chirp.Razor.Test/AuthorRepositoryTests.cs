using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Chirp.Core.DataModels;
using Chirp.Infrastructure;


namespace Chirp.Razor.Test;

public class AuthorRepositoryTests
{

    [Fact]
    public async Task TestGetHighestAuthorID()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedTestDatabase(context);
        
        var aRepository = new AuthorRepository(context);


        var query = context.Authors.ToList();
        
        var numberOfAuthors = query.Count;
    
        //Act
        var hightestID = await aRepository.GetHighestAuthorId();
        
        //Assert
        Assert.Equal(numberOfAuthors, hightestID);
    }
    
    [Fact]
    public async Task TestWriteAuthor()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        var aRepository = new AuthorRepository(context);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>(),
        };
        
        //Act
        await aRepository.WriteAuthor(newAuthor);

        var authors = context.Authors.ToList();
        var author = authors.Last();
        
        //Assert
        Assert.NotEmpty(authors);
        Assert.NotNull(author);
        Assert.Equal(newAuthor, author);
    }
    
    [Fact]
    public async Task TestGetAuthorByName()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        var aRepository = new AuthorRepository(context);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>(),
        };
        
        //Act
        await aRepository.WriteAuthor(newAuthor);

        var author = await aRepository.GetAuthorByName("Test Author");
        
        //Assert
        Assert.NotNull(author);
        Assert.Equal(newAuthor.UserName, author.Name);
    }
    
    [Fact]
    public async Task TestGetAuthorByEmail()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        var aRepository = new AuthorRepository(context);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>(),
        };
        
        //Act
        await aRepository.WriteAuthor(newAuthor);

        var author = await aRepository.GetAuthorByEmail("TestAuthor@gmail.com");
        
        //Assert
        Assert.NotNull(author);
        Assert.Equal(newAuthor.UserName, author.Name);
    }
        
    [Fact]
    public async Task TestGetFollowd()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        var aRepository = new AuthorRepository(context);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>()
            {
                "Roger Histand",
                "Luanna Muro",
                "Wendell Ballan",
                "Nathan Sirmon",
            }
        };
        await aRepository.WriteAuthor(newAuthor);
        
        //Act
        var authorfollows = await aRepository.GetFollowed("Test Author");
        
        //Assert
        Assert.NotEmpty(authorfollows);
        foreach (var followee in authorfollows)
        {
            Assert.Contains(followee, authorfollows);
        }
        
    }
    
    [Fact]
    public async Task TestAddFollows()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        var aRepository = new AuthorRepository(context);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>(),
        };
        await aRepository.WriteAuthor(newAuthor);
        
        //Act
        await aRepository.AddFollows("Test Author", "Roger Histand");
        var authorfollows = await aRepository.GetFollowed("Test Author");
        
        //Assert
        Assert.NotEmpty(authorfollows);
        Assert.Contains("roger histand", authorfollows);
    }
    
    [Fact]
    public async Task TestRemoveFollows()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        
        var aRepository = new AuthorRepository(context);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>()
            {
                "Roger Histand",
                "Luanna Muro",
            }
        };
        await aRepository.WriteAuthor(newAuthor);
        
        //Act
        await aRepository.RemoveFollows("Test Author", "Roger Histand");
        var authorfollows = await aRepository.GetFollowed("Test Author");
        
        //Assert
        Assert.DoesNotContain("Roger histand", authorfollows);
        Assert.Contains("Luanna Muro", authorfollows);
    }
}