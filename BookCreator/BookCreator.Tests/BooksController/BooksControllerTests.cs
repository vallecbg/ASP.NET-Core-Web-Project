using System;
using System.Collections.Generic;
using System.Text;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Books;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;

namespace BookCreator.Tests.BooksController
{
    [TestFixture]
    public class BooksControllerTests
    {
        protected Mock<IBookService> bookService => new Mock<IBookService>();

        [Test]
        public void CreateBook_Should_Return_Error_InvalidInput()
        {
            var book = new BookInputModel
            {
                Author = "Gosho",
                CreatedOn = DateTime.UtcNow,
                Genre = "Ivan",
                BookCoverImage = null,
                Title = null,
                Summary = null
            };

            var controller = new BookCreatorApp.Controllers.BooksController(bookService.Object);
            controller.ModelState.AddModelError("Title", "StringLength");
            var result = controller.CreateBook(book).GetAwaiter().GetResult();

            result.Should()
                .BeOfType<ViewResult>()
                .Which.Model.Should()
                .BeOfType<BookInputModel>();
        }

        [Test]
        public void CreateBook_Should_Return_Error_Invalid_BookCoverImage()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(x => x.ContentType).Returns("image/sss");

            var book = new Mock<BookInputModel>();
            book.Object.BookCoverImage = fileMock.Object;
            book.SetupAllProperties();

            var controller = new BookCreatorApp.Controllers.BooksController(bookService.Object);

            var result = controller.CreateBook(book.Object).GetAwaiter().GetResult();

            result.Should()
                .BeOfType<ViewResult>()
                .Which.ViewData.Values
                .Should()
                .Contain(GlobalConstants.WrongFileType);
        }

        [Test]
        public void Controller_Should_Contains_Authorize()
        {
            typeof(BookCreatorApp.Controllers.BooksController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }
    }
}
