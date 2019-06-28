using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Chapters;
using BookCreator.ViewModels.OutputModels.Chapters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Controllers
{
    [Authorize]
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

        [HttpGet]
        public IActionResult DeleteChapter([FromQuery]string bookId, [FromQuery] string chapterId)
        {
            string username = this.User.Identity.Name;
            this.chapterService.DeleteChapter(bookId, chapterId, username);

            this.ViewData[GlobalConstants.RedirectAfterAction] = bookId;
            return RedirectToAction("Details", "Books", new { id = bookId });
        }

        [HttpGet]
        public IActionResult EditChapter(string id)
        {
            var model = this.chapterService.GetChapterToEdit(id);

            if (model == null)
            {
                return NotFound();
            }

            return this.View(model);
        }

        [HttpPost]
        public IActionResult EditChapter(ChapterEditModel model)
        {
            if (!ModelState.IsValid)
            {
                this.ViewData[GlobalConstants.ChapterLength] = model.Content?.Length ?? 0;
                return this.View(model);
            }

            this.chapterService.EditChapter(model);

            return RedirectToAction("Details", "Books", new {id = model.BookId});
        }
    }
}
