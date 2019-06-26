using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Chapters;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Controllers
{
    public class ChaptersController : Controller
    {
        private readonly IChapterService chapterService;

        public ChaptersController(IChapterService chapterService)
        {
            this.chapterService = chapterService;
        }

        [HttpGet]
        [Route(GlobalConstants.RouteConstants.AddChapterRoute)]
        public IActionResult AddChapter(string bookId)
        {
            this.ViewData[GlobalConstants.BookId] = bookId;
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> AddChapter(ChapterInputModel model)
        {
            if (!ModelState.IsValid)
			{
				this.ViewData[GlobalConstants.ChapterLength] = model.Content?.Length ?? 0;
				this.ViewData[GlobalConstants.BookId] = model.BookId;
				return this.View(model);
			}

            var bookId = await this.chapterService.AddChapter(model);

            return RedirectToAction("Details", "Books", new {id = bookId});
        }
    }
}
