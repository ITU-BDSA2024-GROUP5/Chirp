using System.ComponentModel.DataAnnotations;
using Chirp.Core.DataModels;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account;

public class Register : PageModel
{
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    private readonly IUserStore<Author> _userStore;
    private readonly IUserEmailStore<Author> _emailStore;
    private readonly ILogger<Register> _logger;
    private readonly IAuthorRepository _authorRepository;
    public Register(
        UserManager<Author> userManager,
        IUserStore<Author> userStore,
        SignInManager<Author> signInManager,
        ILogger<Register> logger,
        IAuthorRepository authorRepository)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _logger = logger;
        _authorRepository = authorRepository;
    }
    [BindProperty]
    public InputModel Input { get; set; }
    public string ReturnUrl { get; set; }
    public IList<AuthenticationScheme> ExternalLogins { get; set; }
    public class InputModel
    {
        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string UserName { get; set; }
          
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public async Task OnGetAsync(string returnUrl = null)
    {
        ReturnUrl = returnUrl;
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        if (ModelState.IsValid)
        {
            // Handle teachers
            var predefinedUsers = new Dictionary<string, string>
            {
                { "ropf@itu.dk", "Helge" },
                { "adho@itu.dk", "Adrian" }
            };

            if (predefinedUsers.Any(user => user.Key.Equals(Input.Email)/* && user.Value.Equals(Input.UserName)*/))
            {
                predefinedUsers.TryGetValue(Input.Email, out var name);
                ModelState.AddModelError(string.Empty, $"Hi {name}! We already registered your account for you! Proceed to the login page or register with Github!");
                return Page();
            }
            // End handle teachers
            
            var user = CreateUser();
            //set the username
            
            await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
            user.UserName = Input.UserName;    //add this line....
            user.AuthorId = await _authorRepository.GetHighestAuthorId() + 1;
            
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return Page();
    }
    private Author CreateUser()
    {
        try
        {
            return Activator.CreateInstance<Author>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(Author)}'. " +
                $"Ensure that '{nameof(Author)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }
    private IUserEmailStore<Author> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<Author>)_userStore;
    }
}