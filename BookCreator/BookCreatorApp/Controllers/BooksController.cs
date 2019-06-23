using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly IBookService bookService;

        public BooksController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [HttpGet]
        public IActionResult CreateBook()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook(BookInputModel inputModel)
        {
            bool imageNotNull = inputModel.BookCoverImage != null;
            bool wrongType = false;

            if (imageNotNull)
            {
                var fileType = inputModel.BookCoverImage.ContentType.Split('/')[1];

                wrongType = GlobalConstants.ImageExtensions.Contains(fileType);
            }

            if (!ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            if (!wrongType)
            {
                this.ViewData[GlobalConstants.Error] = GlobalConstants.WrongFileType;
                return this.View(inputModel);
            }

            var id = await this.bookService.CreateBook(inputModel);

            //TODO: change it!!!
            return this.Redirect("/");
            //return RedirectToAction("Details", "Books", new { id });
        }

        [HttpGet]
        public IActionResult AllBooks()
        {
            var model = this.bookService.CurrentBooks(null);
            return this.View(model);
        }

        [HttpPost]
        public IActionResult AllBooks(string genre)
        {
            var model = this.bookService.CurrentBooks(genre);
            return this.View(model);
        }

        [HttpGet]
        [Route(GlobalConstants.RouteConstants.UserBooksRoute)]
        public IActionResult UserBooks(string username)
        {
            var id = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            this.ViewData[GlobalConstants.UsernameHolder] = username;
            var userBooks = this.bookService.UserBooks(id);
            return this.View(userBooks);
        }
    }
}