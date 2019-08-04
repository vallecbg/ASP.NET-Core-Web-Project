using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels;
using BookCreator.ViewModels.InputModels.Books;
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

            if (!wrongType && imageNotNull)
            {
                this.ViewData[GlobalConstants.Error] = GlobalConstants.WrongFileType;
                return this.View(inputModel);
            }

            var id = await this.bookService.CreateBook(inputModel);

            return RedirectToAction("Details", "Books", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteBook(string id)
        {
            var username = this.User.Identity.Name;

            await this.bookService.DeleteBook(id, username);

            return this.RedirectToAction("UserBooks", "Books", new { username });
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


        [HttpGet]
        public IActionResult Details(string id)
        {
            var book = this.bookService.GetBookById(id);
            return this.View(book);
        }

        [HttpPost]
        public IActionResult AddRating(string bookId, double rating)
        {
            var username = this.User.Identity.Name;

            this.bookService.AddRating(bookId, rating, username);

            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        [HttpGet]
        public async Task<IActionResult> Follow(string id)
        {
            var username = this.User.Identity.Name;
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.bookService.Follow(username, userId, id);

            return RedirectToAction("Details", "Books", new { id });
        }

        [HttpGet]
        public async Task<IActionResult> UnFollow(string id)
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.bookService.UnFollow(userId, id);

            return RedirectToAction("Details", "Books", new { id });
        }

        [HttpGet]
        public IActionResult FollowedBooks()
        {
            var name = this.User.Identity.Name;

            var result = this.bookService.FollowedBooks(name);

            return this.View(result);
        }

        [HttpPost]
        public IActionResult FollowedBooks(string genre)
        {
            var name = this.User.Identity.Name;

            var result = this.bookService.FollowedBooksByGenre(name, genre);

            return this.View(result);
        }
    }
}