namespace Chirp.Infrastructure.Data.DTO;


/// <summary>
/// Data Transfer Object for Author to only include the necessary information.
/// </summary>
public class AuthorDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required List<string> Follows { get; set; }
}