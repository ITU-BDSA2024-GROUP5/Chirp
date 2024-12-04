using System.ComponentModel.DataAnnotations;

namespace Chirp.Core.DataModels;


/// <summary>
/// A Cheep is a short message that an Author can write, much like a tweet on Twitter.
/// </summary>
public class Cheep
{
    public required int CheepId{ get; set; }
    public int AuthorId { get; set; }
    [StringLength(160)]
    public required string Text{ get; set; }
    public required DateTime TimeStamp{ get; set; }
    public required Author Author{ get; set; }
}