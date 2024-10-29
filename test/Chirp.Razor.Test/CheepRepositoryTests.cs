using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Oauth.Data;

namespace Chirp.Razor.Test;

public class CheepRepositoryTests
{
    [Fact]
    public async Task TestRead()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);
        
        //Act
        var cheepDTOS = await repository.Read(1);
        var cheepAuthors = new List<string>();
        var cheepTexts = new List<string>();
        cheepDTOS.ForEach(dto => cheepAuthors.Add(dto.Author));
        cheepDTOS.ForEach(dto => cheepTexts.Add(dto.Text));
        
        //Assert
        Assert.Contains("Jacqualine Gilcoine", cheepAuthors);
        Assert.Contains("Starbuck now is what we hear the worst.", cheepTexts);
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
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);
        
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
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);
        
        var query = context.Authors
            .Select(a => a);
        
        var result = await query.ToListAsync();
        var numberOfAuthors = result.Count();
    
        //Act
        var hightestID = await repository.GetHighestAuthorId();
        
        //Assert
        Assert.Equal(numberOfAuthors, hightestID);
    }
    
    [Fact]
    public async Task TestGetHighestCheepID()
    {
        //Arrange
        await using var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync();
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);
        
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
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);

        var newAuthor = new Author()
        {
            Name = "Test Author",
            AuthorId = await repository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
        };
        
        var newCheep = new Cheep()
        {
            CheepId = await repository.GetHighestCheepId() + 1,
            Text = "Test Cheep",
            TimeStamp = DateTime.Now,
            Author = newAuthor,
            AuthorId = newAuthor.AuthorId,
        };
        
        //Act
        await repository.WriteCheep(newCheep);

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
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);

        var newAuthor = new Author()
        {
            Name = "Test Author",
            AuthorId = await repository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
        };
        
        //Act
        await repository.WriteAuthor(newAuthor);

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
        var builder = new DbContextOptionsBuilder<CheepDBContext>().UseSqlite(connection);
        var builder2 = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(connection);
        
        
        await using var context = new CheepDBContext(builder.Options);
        await using var context2 = new ApplicationDbContext(builder2.Options);
        await context.Database.EnsureCreatedAsync();
        await context2.Database.EnsureCreatedAsync();

        var repository = new CheepRepository(context, context2);

        var newAuthor = new Author()
        {
            Name = "Test Author",
            AuthorId = await repository.GetHighestAuthorId() + 1,
            Email = "TestAuthor@gmail.com",
            Cheeps = new List<Cheep>(),
        };
        
        //Act
        await repository.WriteAuthor(newAuthor);

        var authors = context.Authors.ToList();
        var author = await repository.GetAuthorByName("Test Author");
        
        //Assert
        Assert.Equal(newAuthor, author);
    }

    [Fact]
    public void TestWrapInDTO()
    {
        //Arrange
        var newAuthor = new Author()
        {
            Name = "Test Author",
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