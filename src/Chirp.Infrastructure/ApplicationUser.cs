using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure;

public class ApplicationUser : IdentityUser
{
    [StringLength(10, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string UserCreatedUserName { get; set; }
}