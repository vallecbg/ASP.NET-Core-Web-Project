namespace BookCreator.Tests.BookCreatorControllers.CommentsController
{
	using NUnit.Framework;
	using FluentAssertions;
	using BookCreatorApp.Controllers;
	using Microsoft.AspNetCore.Authorization;

	[TestFixture]
	public class CommentsControllerTests
	{
		[Test]
		public void Controller_should_Have_Authorize_Attribute()
		{
			typeof(CommentsController).Should().BeDecoratedWith<AuthorizeAttribute>();
		}
	}
}