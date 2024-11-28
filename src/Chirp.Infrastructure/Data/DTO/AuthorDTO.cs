namespace Chirp.Infrastructure.Data.DTO;


/// <summary>
/// Data Transfer Object for Author to only include the necessary information.
/// </summary>
public class AuthorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> Follows { get; set; }
}