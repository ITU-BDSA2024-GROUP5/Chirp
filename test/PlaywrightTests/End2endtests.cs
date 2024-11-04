using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests;

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
        await Page.GotoAsync("http://localhost:5177/");

        await Page.GetByRole(AriaRole.Link, new() { Name = "Register" }).ClickAsync();

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
        await Page.GotoAsync("http://localhost:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Link, new() { Name = "Hello testuser@gmail.com!" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task CCheepBoxNotVisibleWhenNotLoggedIn()
    {
        await Page.GotoAsync("http://localhost:5177/Public");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).Not.ToBeVisibleAsync();

    }

    [Test]
    public async Task DCheepBoxVisibleWhenLoggedIn()
    {
        await Page.GotoAsync("http://localhost:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Page.GotoAsync("http://localhost:5177/Public");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();

    }

    [Test]
    public async Task EUserCanCheep()
    {
        await Page.GotoAsync("http://localhost:5177/");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Login" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Email" }).FillAsync("testuser@gmail.com");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" }).FillAsync("Nicepassword123#");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();

        await Page.GotoAsync("http://localhost:5177/Public");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Share" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Textbox).FillAsync("This is a cheep");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Share" }).ClickAsync();

        await Expect(Page.GetByText("This is a cheep")).ToBeVisibleAsync();
    }

    [Test]
    public async Task FUserTimeLineHasCheeps()
    {
        await Page.GotoAsync("http://localhost:5177/testuser@gmail.com");
        await Expect(Page.GetByText("There are no cheeps so far.")).Not.ToBeVisibleAsync();
    }

}