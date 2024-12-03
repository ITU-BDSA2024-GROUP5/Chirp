namespace Chirp.Infrastructure.Data.DTO;

/// <summary>
/// Data Transfer Object for Cheep to only include the necessary information.
/// </summary>
public class CheepDTO
{
    public required string Text { get; set; }
    public required string? Author { get; set; }
    public required string TimeStamp { get; set; }
}