using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DataModels;

public class Author : IdentityUser
{
    public required int AuthorId { get; set; }
    public override required string Email { get; set; }
    public override required string UserName { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; }
    public List<string> Follows { get; set; }
}