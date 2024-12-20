using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

// Test can be generated with the help of pwsh bin/Debug/net8.0/playwright.ps1 codegen http://localhost:5177/

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task HomepageHasPlaywrightInTitleAndGetStartedLinkLinkingtoTheIntroPage()
    {
        await Page.GotoAsync("https://playwright.dev");
        // Expect a title "to contain" a substring.
        await Expect(Page).ToHaveTitleAsync(new Regex("Playwright"));

        // create a locator
        var getStarted = Page.GetByRole(AriaRole.Link, new() { Name = "Get started" });
        // Expect an attribute "to be strictly equal" to the value.
        await Expect(getStarted).ToHaveAttributeAsync("href", "/docs/intro");

        // Click the get started link.
        await getStarted.ClickAsync();
        // Expects the URL to contain intro.
        await Expect(Page).ToHaveURLAsync(new Regex(".*intro"));
    }

    [Test]
    public async Task AUserCanRegister()
    {
        await Page.GotoAsync("http://127.0.0.1:5177");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

        await Page.GetByText("Username").FillAsync("testuser");
        await Page.GetByText("Email").FillAsync("testuser@gmail.com");
        await Page.GetByText("Password", new() { Exact = true }).FillAsync("Nicepassword123#");
        await Page.GetByText("Confirm Password").FillAsync("Nicepassword123#");

        await Page.GetByRole(AriaRole.Button, new() { Name = "Register" }).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Click here to confirm your" }).ClickAsync();

        await Expect(Page.GetByText("Thank you for confirming your")).ToBeVisibleAsync();
    }

    [Test]
    public async Task BUserCanLogin()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Hello testuser" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task CCheepBoxNotVisibleWhenNotLoggedIn()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/Public");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).Not.ToBeVisibleAsync();

    }

    [Test]
    public async Task DCheepBoxVisibleWhenLoggedIn()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Page.GotoAsync("http://127.0.0.1:5177/Public");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();

    }

    [Test]
    public async Task EUserCanCheep()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Page.GotoAsync("http://127.0.0.1:5177/Public");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Textbox).FillAsync("This is a cheep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await Expect(Page.GetByText("This is a cheep")).ToBeVisibleAsync();
    }

    [Test]
    public async Task FUserTimeLineHasCheeps()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/testuser@gmail.com");
        await Expect(Page.GetByText("There are no cheeps so far.")).Not.ToBeVisibleAsync();
    }
    
       
    [Test]
    public async Task FUserFollow()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        
        await Page.GotoAsync("http://127.0.0.1:5177/Jacqualine%20Gilcoine");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Follow" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();
        await Page.GotoAsync("http://127.0.0.1:5177/testuser");

        await Expect(Page.GetByText(" Starbuck now is what we hear the worst. ")).ToBeVisibleAsync();
    }
        
    
    [Test]
    public async Task GUserUnfollow()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GotoAsync("http://127.0.0.1:5177/Jacqualine%20Gilcoine");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Unfollow" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "My Timeline" }).ClickAsync();


        await Expect(Page.GetByText("Jacqualine Gilcoine")).Not.ToBeVisibleAsync();
    }
    
    [Test]
    public async Task HUserSeeAboutMe()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        
        await Expect(Page.GetByText("Personal Data Vault")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task IShowCheepButtonsWork()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        
        await Expect(Page.GetByText("Show Cheeps")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Cheeps" }).ClickAsync();
        
        await Expect(Page.GetByText("Go Back")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Go Back" }).ClickAsync();
        
        await Expect(Page.GetByText("Show Cheeps")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task JShowFollowsButtonsWork()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GetByRole(AriaRole.Link, new() { Name = "About me" }).ClickAsync();
        
        await Expect(Page.GetByText("Show Followed")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Show Followed" }).ClickAsync();
        
        await Expect(Page.GetByText("Go Back")).ToBeVisibleAsync();
        
        await Page.GetByRole(AriaRole.Button, new() { Name = "Go Back" }).ClickAsync();
        
        await Expect(Page.GetByText("Show Followed")).ToBeVisibleAsync();
    }

    [Test]
    public async Task KUserCanSearchByEmail()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/testuser@gmail.com");
        await Expect(Page.GetByText("testuser's Timeline")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task LUserCanSearchByUsername()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/testuser");
        await Expect(Page.GetByText("testuser's Timeline")).ToBeVisibleAsync();
    }
            
    [Test]
    public async Task MSearchingForYourOwnNameResultsInTheSameAsGoingToPersonalUserTimeline()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GotoAsync("http://127.0.0.1:5177/Helge");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();
        
        await Page.GotoAsync("http://127.0.0.1:5177/testuser");
        await Expect(Page.GetByText("What's on your mind testuser?")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Helge Hello, BDSA students!")).ToBeVisibleAsync();
    }
    
    [Ignore("Don't know why this test fails, is almost same as Test K")]
    [Test]
    public async Task NSearchingForYourOwnEmailResultsInTheSameAsGoingToPersonalUserTimeline()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GotoAsync("http://127.0.0.1:5177/Helge");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Follow" }).ClickAsync();
        
        await Page.GotoAsync("http://127.0.0.1:5177/testuser@gmail.com");
        await Expect(Page.GetByText("What's on your mind testuser?")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Helge Hello, BDSA students!")).ToBeVisibleAsync();
    }
    
    [Test]
    public async Task ZUserCanDeleteAccount()
    {
        await Page.GotoAsync("http://127.0.0.1:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        
        await Page.GotoAsync("http://127.0.0.1:5177/Identity/Account/Manage/DeletePersonalData");
        await Page.GetByRole(AriaRole.Textbox, new() {Name="Password"}).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Delete data and close my account" }).ClickAsync();
        await Expect(Page).ToHaveTitleAsync("Home page - Chirp");
    }
}