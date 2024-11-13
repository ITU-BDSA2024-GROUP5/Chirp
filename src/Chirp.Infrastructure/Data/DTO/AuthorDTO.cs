namespace Chirp.Infrastructure.Data.DTO;

public class AuthorDTO
{
    public string Name { get; set; }
    public string Email { get; set; }
    
    public List<string> Follows { get; set; }
}