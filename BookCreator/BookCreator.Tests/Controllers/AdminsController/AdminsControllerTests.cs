using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.OutputModels.Books;
using BookCreatorApp.Controllers;
using Castle.Core.Internal;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace BookCreator.Tests.Controllers.AdminsController
{
    [TestFixture]
    public class AdminsControllerTests
    {
        protected Mock<IAdminService> adminService => new Mock<IAdminService>();
        protected Mock<IBookService> bookService => new Mock<IBookService>();


        [Test]
        public async Task DeleteBook_Error_MissingRole()
        {
            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
            var adminsController = new BookCreatorApp.Areas.Administration.Controllers.AdminsController(adminService.Object, bookService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user.Object }
                }
            };

            var id = "1";
            var result = await adminsController.DeleteBook(id);

            Assert.AreEqual(((RedirectToActionResult)result).ActionName, nameof(HomeController.Error));
        }

        [Test]
        public async Task DeleteBook_Success_RedirectTo_AllBooks()
        {
            var username = "admintest";
            var user = new Mock<ClaimsPrincipal>();
            user.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
            user.Setup(x => x.Identity.Name).Returns(username);

            var adminsController = new BookCreatorApp.Areas.Administration.Controllers.AdminsController(adminService.Object, bookService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user.Object }
                }
            };

            string id = "1";
            var result = await adminsController.DeleteBook(id);

            Assert.AreEqual(((RedirectToActionResult)result).ActionName, nameof(BookCreatorApp.Areas.Administration.Controllers.AdminsController.CurrentBooks));
        }

        [Test]
        public void CurrentGenres_Should_Return_Correct_View_With_Empty_String_On_Post()
        {
            bookService.Setup(x => x.Genres()).Returns(new List<BookGenreOutputModel>()
            {
                new BookGenreOutputModel()
                {
                    GenreName = "Horror"
                },
                new BookGenreOutputModel()
                {
                    GenreName = "Fantasy"
                }
            });

            var adminController = new BookCreatorApp.Areas.Administration.Controllers.AdminsController(adminService.Object, bookService.Object);

            var result = adminController.CurrentGenres(null);

            result.As<ViewResult>().ViewData
                .ContainsKey(GlobalConstants.Error)
                .Equals(GlobalConstants.NullName);
        }

        [Test]
        public void CurrentGenres_Should_Redirect_Back_When_Genre_Exists()
        {
            bookService.Setup(x => x.Genres()).Returns(new List<BookGenreOutputModel>()
            {
                new BookGenreOutputModel()
                {
                    GenreName = "Horror"
                },
                new BookGenreOutputModel()
                {
                    GenreName = "Fantasy"
                }
            });

            var genreName = "asd";

            adminService.Setup(x => x.AddGenre(genreName)).Returns(GlobalConstants.Failed);

            var adminController =
                new BookCreatorApp.Areas.Administration.Controllers.AdminsController(adminService.Object,
                    bookService.Object);

            var result = adminController.CurrentGenres(genreName);

            string expected = string.Join(GlobalConstants.AlreadyExistsInDb, genreName);
            result.As<ViewResult>().ViewData.ContainsKey(GlobalConstants.Error).Equals(expected);
        }

        [Test]
        public void AdminsController_Have_Authorize_And_Should_Be_Accessed_Only_From_Admin()
        {
            var result = typeof(BookCreatorApp.Areas.Administration.Controllers.AdminsController).GetAttribute<AuthorizeAttribute>();

            var roles = $"{GlobalConstants.Admin}";

            Assert.AreEqual(result.Roles, roles);
        }
    }
}
