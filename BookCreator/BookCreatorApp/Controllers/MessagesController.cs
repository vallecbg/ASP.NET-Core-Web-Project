using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Messages;
using BookCreator.ViewModels.OutputModels.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookCreatorApp.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IMessageService messageService;
        private readonly ICommentService commentService;

        public MessagesController(IMessageService messageService, ICommentService commentService)
        {
            this.messageService = messageService;
            this.commentService = commentService;
        }

        [HttpPost]
        public IActionResult SendMessage(MessageInputModel inputModel)
        {
            if (string.IsNullOrWhiteSpace(inputModel.Message))
            {
                this.TempData[GlobalConstants.Error] = GlobalConstants.EmptyMessage;
                return RedirectToAction("Profile", "Users", new { username = inputModel.ReceiverName });
            }

            this.messageService.SendMessage(inputModel);

            return RedirectToAction("Profile", "Users", new { username = inputModel.ReceiverName });
        }

        [HttpGet]
        public IActionResult UserMessages(string userId)
        {
            var sentMessages = this.messageService.GetSentMessages(userId);
            var receivedMessages = this.messageService.GetReceivedMessages(userId);
            var comments = this.commentService.GetComments(userId);

            var userMessagesModel = new UserMessagesOutputModel
            {
                SentMessages = sentMessages,
                ReceivedMessages = receivedMessages,
                Comments = comments
            };

            return this.View(userMessagesModel);
        }

        [HttpGet]
        public IActionResult SeenMessage(string messageId)
        {
            this.messageService.MarkMessageAsSeen(messageId);

            return this.RedirectToAction("UserMessages", "Messages", new{userId = User.FindFirstValue(ClaimTypes.NameIdentifier)});
        }

        [HttpGet]
        public IActionResult DeleteMessage(string messageId)
        {
            this.messageService.DeleteMessage(messageId);

            return this.RedirectToAction("UserMessages", "Messages", new { userId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }

        [HttpGet]
        public IActionResult DeleteComment(string id)
        {
            this.commentService.DeleteComment(id);

            return this.RedirectToAction("UserMessages", "Messages", new { userId = User.FindFirstValue(ClaimTypes.NameIdentifier) });
        }
    }
}
