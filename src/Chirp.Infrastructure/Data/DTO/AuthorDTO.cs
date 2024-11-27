namespace Chirp.Infrastructure.Data.DTO;

public class AuthorDTO
{
    public AuthorDTO(string? name,string? email)
    {
        this.Name = name;
        this.Email = email;
        Follows = new List<string>();
    }
    
    
    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<string> Follows { get; set; }
}