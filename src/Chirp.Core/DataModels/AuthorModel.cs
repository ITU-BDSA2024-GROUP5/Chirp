using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DataModels;

public class Author : IdentityUser
{
    public required int AuthorId { get; set; }
    public new required string Email { get; set; }
    public override required string UserName { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; }
}