using System;
using BookCreator.Services.Interfaces;
using BookCreator.ViewModels.InputModels.Chapters;
using BookCreator.ViewModels.OutputModels.Chapters;
using Moq;
using NUnit.Framework;
using BookCreatorApp.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCreator.Tests.Controllers.ChaptersController
{
    [TestFixture]
    public class ChaptersControllerTests
    {
        [Test]
        public void AddChapter_Should_Return_Error_InvalidInput()
        {
            var chapter = new ChapterInputModel
            {
                BookId = "1",
                Author = "usertest",
                Content = null,
                CreatedOn = DateTime.UtcNow,
                Title = "title"
            };

            var chapterService = new Mock<IChapterService>();

            var controller = new BookCreatorApp.Controllers.ChaptersController(chapterService.Object);
            controller.ModelState.AddModelError("Content", "StringLength");

            var result = controller.AddChapter(chapter);

            result.Should()
                .BeOfType<ViewResult>()
                .Which.Model.Should()
                .BeOfType<ChapterInputModel>();
        }

        [Test]
        public void AddChapter_Valid()
        {
            var chapter = new ChapterInputModel()
            {
                BookId = "1",
                Author = "usertest",
                CreatedOn = DateTime.UtcNow,
                Content = null,
                Title = "title"
            };

            var chapterService = new Mock<IChapterService>();

            var controller = new BookCreatorApp.Controllers.ChaptersController(chapterService.Object);

            var result = controller.AddChapter(chapter);

            string redirectToActionName = "Details";
            string redirectTo = "Books";
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(redirectToActionName);
            result.Should().BeOfType<RedirectToActionResult>().Which.ControllerName.Should().Be(redirectTo);
            result.Should().BeOfType<RedirectToActionResult>()
                .Which.RouteValues.Values.Count
                .Should().Be(1).And.Subject
                .Should().Be(1);
        }

        [Test]
        public void EditChapter_Return_NotFound()
        {
            var chapterService = new Mock<IChapterService>();

            var controller = new BookCreatorApp.Controllers.ChaptersController(chapterService.Object);

            string id = "unknown";
            chapterService.Setup(x => x.GetChapterToEdit(id)).Returns(() => null);
            var result = controller.EditChapter(id);

            result.Should().BeOfType<NotFoundResult>();
        }

        [Test]
        public void EditChapter_Should_Return_Error_InvalidInput()
        {
            var chapter = new ChapterEditModel
            {
                BookId = "1",
                Author = "usertest",
                Content = null,
                CreatedOn = DateTime.UtcNow,
                Title = "title",
            };

            var chapterService = new Mock<IChapterService>();

            var controller = new BookCreatorApp.Controllers.ChaptersController(chapterService.Object);
            controller.ModelState.AddModelError("Content", "StringLength");

            var result = controller.EditChapter(chapter);

            result.Should()
                .BeOfType<ViewResult>()
                .Which.Model.Should()
                .BeOfType<ChapterEditModel>();
        }


        public void Controller_Should_Contains_Authorize()
        {
            typeof(BookCreatorApp.Controllers.ChaptersController)
                .Should()
                .BeDecoratedWith<AuthorizeAttribute>();
        }
    }
}
