using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DataModels;

public class Author : IdentityUser
{
    public required int AuthorId { get; set; }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; }
}