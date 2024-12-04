using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Chirp.Core.DataModels;
using Chirp.Infrastructure;


namespace Chirp.Razor.Test;

public class CheepRepositoryTests
{
    [Fact]
    public async Task TestRead()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedTestDatabase(context);

        var cheepRepository = new CheepRepository(context);
        
        //Act
        var cheeps = await cheepRepository.Read(1);
        
        Assert.NotEmpty(cheeps);
    }

    [Fact]
    public async Task TestReadAllCheeps()
    {
        {
            //Arrange
            await using var connection = new SqliteConnection("Filename=:memory:");
            await connection.OpenAsync();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);


            await using var context = new ApplicationDbContext(builder.Options);
            await context.Database.EnsureCreatedAsync();
            await DbInitializer.SeedTestDatabase(context);

            var cheepRepository = new CheepRepository(context);

            //Act
            var cheeps = await cheepRepository.ReadAllCheeps("Jacqualine Gilcoine");

            foreach (var dto in cheeps)
            {
                Assert.Equal("Jacqualine Gilcoine", dto.Author);
            }
        }
    }

    [Theory]
    [InlineData("Jacqualine Gilcoine")]
    [InlineData("Roger Histand")]
    [InlineData("Luanna Muro")]
    [InlineData("Wendell Ballan")]
    [InlineData("Nathan Sirmon")]
    [InlineData("Adrian")]
    [InlineData("Helge")]
    public async Task TestReadByAuthor(string author)
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedTestDatabase(context);

        var repository = new CheepRepository(context);
        
        //Act
        var cheepDTOS = await repository.ReadByAuthor(0, author);
       
        //Assert
        foreach (var dto in cheepDTOS)
        {
            Assert.Equal(author, dto.Author);
        }
    }
    
    [Fact]
    public async Task TestGetHighestCheepID()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedTestDatabase(context);

        var repository = new CheepRepository(context);

        var query = context.Cheeps.ToList();
        
        var numberOfCheeps = query.Count;

        //Act
        var hightestID = await repository.GetHighestCheepId();
        
        //Assert
        Assert.Equal(numberOfCheeps, hightestID);
    }

    [Fact]
    public async Task TestWriteCheep()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedTestDatabase(context);

        var cRepository = new CheepRepository(context);
        var aRepository = new AuthorRepository(context);
        

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>(),
        };
        
        var newCheep = new Cheep()
        {
            CheepId = await cRepository.GetHighestCheepId() + 1,
            Text = "Test Cheep",
            TimeStamp = DateTime.Now,
            Author = newAuthor,
            AuthorId = newAuthor.AuthorId,
        };
        
        //Act
        await cRepository.WriteCheep(newCheep);

        var cheeps = context.Cheeps.ToList();
        var cheep = cheeps.Last();
        
        //Assert
        Assert.Equal(newCheep, cheep);
    }
    
    

    [Fact]
    public void TestWrapInDTO()
    {
        //Arrange
        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
            Follows = new List<string>(),
        };
        
        var newCheep = new Cheep()
        {
            CheepId = 1,
            Text = "Test Cheep",
            TimeStamp = DateTime.Now,
            Author = newAuthor,
            AuthorId = newAuthor.AuthorId,
        };
        
        var list = new List<Cheep>();
        list.Add(newCheep);
        
        //Act
        var dtoList = CheepRepository.WrapInDto(list);
        var cheepDTO = dtoList.First();
        
        //Assert
        Assert.Equal(typeof(CheepDto), cheepDTO.GetType());
    }
    
    [Fact]
    public async Task TestGetCheepsFollowedByAuthor()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();
        await DbInitializer.SeedTestDatabase(context);

        var repository = new CheepRepository(context);

        var author = context.Authors.First(a => a.UserName.Equals("Jacqualine Gilcoine"));
        var follows = author.Follows = new List<string>()
        {
            "Roger Histand",
            "Luanna Muro",
            "Wendell Ballan",
            "Nathan Sirmon",
        };

        var authors = new List<string>()
        {
            "Jacqualine Gilcoine",
            "Roger Histand",
            "Luanna Muro",
            "Wendell Ballan",
            "Nathan Sirmon",
        };
        
        
        //Act
        var cheepers = await repository.GetCheepsFollowedByAuthor(0, author.UserName, follows);
        
        //Assert
        Assert.NotNull(author);
        Assert.NotEmpty(cheepers);
        foreach (var cheep in cheepers)
        {
            Assert.Contains(cheep.Author, authors);
        }
    }
}