
using Microsoft.AspNetCore.Identity;

namespace Chirp.Core.DataModels;


/// <summary>
/// Represents a user for the Chirp application. Is called author because a user will be an author of cheeps.
/// Extends IdentityUser to work with ASP.NET Identity.
/// </summary>
public class Author : IdentityUser
{
    public required int AuthorId { get; set; }
    public override required string Email { get; set; }
    public override required string UserName { get; set; }
    public required ICollection<Cheep> Cheeps { get; set; }
    public required List<string> Follows { get; set; }
}