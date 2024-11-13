// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using Chirp.Core.DataModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account;

public class ExternalLoginModel : PageModel
{
    private readonly SignInManager<Author> _signInManager;
    private readonly UserManager<Author> _userManager;
    private readonly ILogger<ExternalLoginModel> _logger;
    private readonly IAuthorRepository _authorRepository;
    private readonly IUserStore<Author> _userStore;
    /*private readonly IUserEmailStore<Author> _emailStore;*/

    public ExternalLoginModel(
        SignInManager<Author> signInManager,
        UserManager<Author> userManager,
        ILogger<ExternalLoginModel> logger,
        IAuthorRepository authorRepository,
        IUserStore<Author> userStore)
        /*IUserEmailStore<Author> emailStore*/
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _authorRepository = authorRepository;
        _userStore = userStore;
       // _emailStore = GetEmailStore();
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string LoginProvider { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
    }

    public IActionResult OnGetAsync()
    {
        return RedirectToPage("./Login");
    }
    
    // Redirect to github for login
    public IActionResult OnPost(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider.
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }
    
    // Get callback from github and sign in or redirect to register
    public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
    {
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login");
        }
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToPage("./Login");
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            var props = new AuthenticationProperties();
            props.StoreTokens(info.AuthenticationTokens);
            await _signInManager.SignInAsync(user, props, info.LoginProvider);

            _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
            return LocalRedirect("/Public");
        }
        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }
        else
        {
            ReturnUrl = returnUrl;
            LoginProvider = info.LoginProvider;
            // if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            // {
            //     Input = new InputModel
            //     {
            //         Email = info.Principal.FindFirstValue(ClaimTypes.Email)
            //     };
            // }
            // if (info.Principal.HasClaim(c => c.Type == "urn:github:login"))
            // {
            //     Input.Username = info.Principal.FindFirstValue("urn:github:login");
            // }
            return Page();
        }
    }
    
    //Confirmation of external login and registration if necessary
    public async Task<IActionResult> OnPostConfirmationAsync()
    {
        if (ModelState.IsValid)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }

            var user = new Author
            {
                UserName = Input.Username,
                Email = Input.Email,
                Cheeps = new List<Cheep>(),
                AuthorId = await _authorRepository.GetHighestAuthorId() + 1
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    //await _userManager.AddClaimAsync(user, info.Principal.FindFirst(ClaimTypes.Gender));

                    // var props = new AuthenticationProperties();
                    // props.StoreTokens(info.AuthenticationTokens);
                    //
                    // await _signInManager.SignInAsync(user, props, authenticationMethod: info.LoginProvider);
                    // _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                    // return LocalRedirect(Url.RouteUrl(returnUrl));
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
                    }
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect("/Public");
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