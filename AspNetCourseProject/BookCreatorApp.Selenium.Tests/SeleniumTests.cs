namespace BookCreatorApp.Selenium.Tests
{
	using FluentAssertions;
	using Xunit;
	using OpenQA.Selenium;
	using OpenQA.Selenium.Chrome;
	using OpenQA.Selenium.Remote;

	public class SeleniumTests : IClassFixture<SeleniumServerFactory<Startup>>
	{
		private readonly SeleniumServerFactory<Startup> server;
		private readonly IWebDriver browser;

		public SeleniumTests(SeleniumServerFactory<Startup> server)
		{
			this.server = server;
			server.CreateClient();
			var opts = new ChromeOptions();
			opts.AddArgument("--headless"); //Optional, comment this out if you want to SEE the browser window
			opts.AddArgument("no-sandbox");
			this.browser = new RemoteWebDriver(opts);
		}

		[Fact]
		public void LoadTheMainPageAndCheckTitle()
		{
			browser.Navigate().GoToUrl(server.RootUri);
			Assert.StartsWith("Home Page", browser.Title);
		}

		[Fact]
		public void LoadMainPageAndThenGoToLoginPageAndLogin()
		{
			browser.Navigate().GoToUrl(server.RootUri);
			browser.FindElement(By.LinkText("Login")).Click();
			browser.FindElement(By.Name("Nickname")).SendKeys("ThatAdmin");
			browser.FindElement(By.Name("Password")).SendKeys("admin");
			browser.FindElement(By.Name("LoginButton")).Submit();

			var hello = browser.FindElement(By.Id("hello")).Text;

			string introduction = "Hello AppAdmin!";
			hello.Should().Be(introduction);
			Assert.StartsWith("LoggedHome", browser.Title);
		}

		[Fact]
		public void LoginAsAdminAndCreateStory()
		{
			browser.Navigate().GoToUrl(server.RootUri);
			browser.FindElement(By.LinkText("Login")).Click();
			browser.FindElement(By.Name("Nickname")).SendKeys("ThatAdmin");
			browser.FindElement(By.Name("Password")).SendKeys("admin");
			browser.FindElement(By.Name("LoginButton")).Submit();
			browser.FindElement(By.Id("MyStories")).Click();
			browser.FindElement(By.LinkText("Add Story")).Click();
			browser.FindElement(By.Name("Title")).SendKeys("SeleniumTestStory");
			browser.FindElement(By.Name("Genre")).SendKeys("fantasy");
			browser.FindElement(By.Name("StoryImage"));
			browser.FindElement(By.Name("Summary")).SendKeys("Selenium story summary");
			browser.FindElement(By.Id("CreateStoryButton")).Submit();
			var result = browser.Title.Split(" ")[0];

			result.Should().Be("Details");
		}

		//[Fact]
		//public void GetOnDetailsStoryPageAndRateAndFollowStoryInThisOrderShouldNotCreateBug()
		//{
		//	browser.Navigate().GoToUrl(server.RootUri);
		//	browser.FindElement(By.LinkText("Login")).Click();
		//	browser.FindElement(By.Name("Nickname")).SendKeys("ThatAdmin");
		//	browser.FindElement(By.Name("Password")).SendKeys("admin");
		//	browser.FindElement(By.Name("LoginButton")).Submit();
		//	browser.FindElement(By.Id("MyStories")).Click();
		//	browser.FindElement(By.LinkText("Add Story")).Click();
		//	browser.FindElement(By.Name("Title")).SendKeys("SeleniumTestStoryТеstRating");
		//	browser.FindElement(By.Name("Genre")).SendKeys("fantasy");
		//	browser.FindElement(By.Name("StoryImage"));
		//	browser.FindElement(By.Name("Summary")).SendKeys("Selenium story summary");
		//	browser.FindElement(By.Id("CreateStoryButton")).Submit();
		//	browser.FindElement(By.LinkText("Logout")).Click();

		//	browser.FindElement(By.LinkText("Login")).Click();
		//	browser.FindElement(By.Name("Nickname")).SendKeys("user");
		//	browser.FindElement(By.Name("Password")).SendKeys("123");
		//	browser.FindElement(By.Name("LoginButton")).Submit();

		//	var result = browser.Title.Split(" ")[0];
		//	result.Should().Be("Details");
		//}
	}
}