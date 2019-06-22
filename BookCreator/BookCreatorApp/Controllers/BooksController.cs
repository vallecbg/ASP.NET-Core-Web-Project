using System;
using System.Collections.Generic;
using System.Linq;
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
            //bool imageNotNull = inputModel.ImageUrl != null;
            //bool wrongType = false;

            //if (imageNotNull)
            //{
            //    var fileType = inputModel.StoryImage.ContentType.Split('/')[1];

            //    wrongType = GlobalConstants.imageFormat.Contains(fileType);
            //}

            if (!ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            //if (!wrongType)
            //{
            //    this.ViewData[GlobalConstants.Error] = GlobalConstants.WrongFileType;
            //    return this.View(inputModel);
            //}

            var id = await this.bookService.CreateBook(inputModel);

            return this.Redirect("/");
            //return RedirectToAction("Details", "Books", new { id });
        }
    }
}