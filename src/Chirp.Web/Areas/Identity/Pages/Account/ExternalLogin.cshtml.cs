// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
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

    public ExternalLoginModel(
        SignInManager<Author> signInManager,
        UserManager<Author> userManager,
        ILogger<ExternalLoginModel> logger,
        IAuthorRepository authorRepository)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _authorRepository = authorRepository;
    }

    public string LoginProvider { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public IActionResult OnGetAsync()
    {
        return RedirectToPage("./Login");
    }
    
    // Redirect to github for login by ChallengeResult
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
        // Get authenticated user info
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return RedirectToPage("./Login");
        }
        
        // Check if there is already a user with the same email
        var useremail = info.Principal.Claims.First(c => c.Type == ClaimTypes.Email)?.Value;
        var userfound = await _userManager.FindByEmailAsync(useremail);
        if (userfound != null)
        {
            // add the github login to the existing user
            var userresult = await _userManager.AddLoginAsync(userfound, info);
            var props = new AuthenticationProperties();
            props.StoreTokens(info.AuthenticationTokens);
            // sign in the user and redirect to the public page
            await _signInManager.SignInAsync(userfound, props, info.LoginProvider);
            return LocalRedirect("/Public");
        }
        
        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {   
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            var props = new AuthenticationProperties();
            props.StoreTokens(info.AuthenticationTokens);
            await _signInManager.SignInAsync(user, props, info.LoginProvider);
            foreach (var claim in info.Principal.Claims)
            {
                Console.WriteLine(claim);
            }
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
            return Page();
        }
    }
    
    //Confirmation of external login and registration if necessary
    public async Task<IActionResult> OnPostConfirmationAsync(string? provider)
    {
        if (ModelState.IsValid)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                throw new ApplicationException("Error loading external login information during confirmation.");
            }
            
            // if (info.Principal.Claims.First(c => c.Type == ClaimTypes.Email)?.Value.Equals("nickyye@hotmail.dk") ?? false)
            // {
            //     handleTeacher(provider);
            //     return LocalRedirect("/Public");
            // }
            var user = new Author
            {
                UserName = info.Principal.Identity.Name,
                Email = info.Principal.Claims.First(c => c.Type == ClaimTypes.Email)?.Value,
                Cheeps = new List<Cheep>(),
                AuthorId = await _authorRepository.GetHighestAuthorId() + 1
            };

            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    var props = new AuthenticationProperties();
                    props.StoreTokens(info.AuthenticationTokens);
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        await _userManager.ConfirmEmailAsync(user, emailConfirmationToken);
                    }
                    await _signInManager.SignInAsync(user, props, authenticationMethod: info.LoginProvider);
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
    
    private async void handleTeacher(string provider)
    {
        var user = await _userManager.FindByEmailAsync("nickyye@hotmail.dk");
        Console.WriteLine("find user with email"+user);
        Console.WriteLine("await get"+await _userManager.GetUserIdAsync(user));
        var info = await _signInManager.GetExternalLoginInfoAsync();
        var props = new AuthenticationProperties();
        props.StoreTokens(info.AuthenticationTokens);
        await _signInManager.SignInAsync(user, props, authenticationMethod: info.LoginProvider);
        
        var info2 = await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
        Console.WriteLine(info2);
        var result = await _userManager.AddLoginAsync(user, info2);
        if (!result.Succeeded)
        {
            throw new ApplicationException($"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
        }
    }
}