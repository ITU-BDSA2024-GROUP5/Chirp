using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure.Data;
using Chirp.Infrastructure.Data.DTO;
using Chirp.Infrastructure.Repositories;
using Chirp.Infrastructure.Services;
using Chirp.Core.DataModels;


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

        var cRepository = new CheepRepository(context);
        var aRepository = new AuthorRepository(context);

        var service = new ChirpService(cRepository, aRepository);
        
        //Act
        var cheepDTOS = await cRepository.Read(0);
        var cheepAuthors = new List<string>();
        var cheepTexts = new List<string>();
        cheepDTOS.ForEach(dto => cheepAuthors.Add(dto.Author));
        cheepDTOS.ForEach(dto => cheepTexts.Add(dto.Text));
        
        //Assert
        //Assert.Contains("Jacqualine Gilcoine", cheepAuthors);
        //Assert.Contains("Starbuck now is what we hear the worst.", cheepTexts);
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
    public async Task TestGetHighestAuthorID()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();

        var cRepository = new CheepRepository(context);
        var aRepository = new AuthorRepository(context);

        var service = new ChirpService(cRepository, aRepository);

        var query = context.Authors
            .Select(a => a);
        
        var result = await query.ToListAsync();
        var numberOfAuthors = result.Count();
    
        //Act
        var hightestID = await aRepository.GetHighestAuthorId();
        
        //Assert
        Assert.Equal(numberOfAuthors, hightestID);
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

        var repository = new CheepRepository(context);
        
        var query = context.Cheeps
            .Select(a => a);
        
        var result = await query.ToListAsync();
        var numberOfCheeps = result.Count();

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

        var cRepository = new CheepRepository(context);
        var aRepository = new AuthorRepository(context);

        var service = new ChirpService(cRepository, aRepository);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
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
    public async Task TestWriteAuthor()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new ApplicationDbContext(builder.Options);
        await context.Database.EnsureCreatedAsync();

        var cRepository = new CheepRepository(context);
        var aRepository = new AuthorRepository(context);

        var service = new ChirpService(cRepository, aRepository);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
        };
        
        //Act
        await aRepository.WriteAuthor(newAuthor);

        var authors = context.Authors.ToList();
        var author = authors.Last();
        
        //Assert
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

        var cRepository = new CheepRepository(context);
        var aRepository = new AuthorRepository(context);

        var service = new ChirpService(cRepository, aRepository);

        var newAuthor = new Author()
        {
            UserName = "Test Author",
            AuthorId = await aRepository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
        };
        
        //Act
        await aRepository.WriteAuthor(newAuthor);

        var author = await aRepository.GetAuthorByName("Test Author");
        
        //Assert
        Assert.Equal(newAuthor.UserName, author.Name);
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
        var dtoList = CheepRepository.WrapInDTO(list);
        var cheepDTO = dtoList.First();
        
        //Assert
        Assert.Equal(typeof(CheepDTO), cheepDTO.GetType());
    }
}