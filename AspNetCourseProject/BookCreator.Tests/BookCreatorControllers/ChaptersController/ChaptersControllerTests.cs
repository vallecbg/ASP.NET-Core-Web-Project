namespace BookCreator.Tests.BookCreatorControllers.ChaptersController
{
	using Moq;
	using System;
	using NUnit.Framework;
	using FluentAssertions;
	using Services.Interfaces;
	using ViewModels.InputModels;
	using Microsoft.AspNetCore.Mvc;
	using BookCreatorApp.Controllers;
	using ViewModels.OutputModels.Chapters;
	using Microsoft.AspNetCore.Authorization;

	[TestFixture]
	public class ChaptersControllerTests
	{
		[Test]
		public void AddChapter_Should_ReturnError_AddChapter()
		{
			var chapter = new ChapterInputModel()
			{
				StoryId = 1,
				Author = "SomeAuthor",
				Content = null,
				CreatedOn = DateTime.UtcNow,
				Title = "SomeTitle"
			};
			var chapterService = new Mock<IChapterService>();

			var controller = new ChaptersController(chapterService.Object);
			controller.ModelState.AddModelError("Content", "StringLength");

			var result = controller.AddChapter(chapter);

			result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<ChapterInputModel>();
		}

		[Test]
		public void AddChapter_Should_AddChapter()
		{
			var chapter = new ChapterInputModel()
			{
				StoryId = 1,
				Author = "SomeAuthor",
				Content = null,
				CreatedOn = DateTime.UtcNow,
				Title = "SomeTitle"
			};
			var chapterService = new Mock<IChapterService>();

			var controller = new ChaptersController(chapterService.Object);

			var result = controller.AddChapter(chapter);

			int storyId = chapter.StoryId;
			string redirectActionName = "Details";
			string controlerToRedirectTo = "Stories";
			result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(redirectActionName);
			result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be(controlerToRedirectTo);
			result.Should().BeOfType<RedirectToActionResult>()
				.Which.RouteValues.Values.Count
				.Should().Be(1).And.Subject
				.Should().Be(storyId);
		}

		[Test]
		public void EditChapter_Should_Return_Not_Found()
		{
			var chapterService = new Mock<IChapterService>();

			var controller = new ChaptersController(chapterService.Object);

			int id = 0;
			chapterService.Setup(x => x.GetChapterToEditById(id)).Returns(() => null);
			var result = controller.EditChapter(id);

			result.Should().BeOfType<NotFoundResult>();
		}

		[Test]
		public void EditChapter_Should_ReturnError_AddChapter()
		{
			var chapter = new ChapterEditModel
			{
				StoryId = 1,
				Author = "SomeAuthor",
				Content = null,
				CreatedOn = DateTime.UtcNow,
				Title = "SomeTitle"
			};
			var chapterService = new Mock<IChapterService>();

			var controller = new ChaptersController(chapterService.Object);
			controller.ModelState.AddModelError("Content", "StringLength");

			var result = controller.EditChapter(chapter);

			result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<ChapterEditModel>();
		}

		[Test]
		public void Controller_should_Have_Authorize_Attribute()
		{
			typeof(ChaptersController).Should().BeDecoratedWith<AuthorizeAttribute>();
		}
	}
}