namespace Chirp.Razor.Pages;

using Microsoft.EntityFrameworkCore;

public interface ICheepRepository
{
    public Task<List<Cheep>> Read(int page);
    public Task<List<Cheep>> ReadByAuthor(int page, string author);
    public void Write(Cheep cheep);
}