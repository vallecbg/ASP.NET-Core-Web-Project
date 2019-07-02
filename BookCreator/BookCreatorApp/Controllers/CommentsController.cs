using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService commentService;

        public CommentsController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost]
        public IActionResult AddComment(CommentInputModel model)
        {
            if (!ModelState.IsValid)
            {
                this.TempData[GlobalConstants.Error] = GlobalConstants.CommentsLength;
                return RedirectToAction("Details", "Books", new { id = model.BookId });
            }

            this.commentService.AddComment(model);

            return RedirectToAction("Details", "Books", new {id = model.BookId});
        }

        [HttpGet]
        public IActionResult DeleteComment(string bookId, string id)
        {
            this.commentService.DeleteComment(id);

            return RedirectToAction("Details", "Books", new {id = bookId});
        }

        [HttpGet]
        public IActionResult DeleteAllComments(string username)
        {
            this.commentService.DeleteAllComments(username);

            //TODO: Replace this
            return Redirect("/");
        }
    }
}
