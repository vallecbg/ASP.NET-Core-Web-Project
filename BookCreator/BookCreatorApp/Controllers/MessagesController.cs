using System;
using System.Collections.Generic;
using System.Linq;
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

        public MessagesController(IMessageService messageService)
        {
            this.messageService = messageService;
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

            var userMessagesModel = new UserMessagesOutputModel
            {
                SentMessages = sentMessages,
                ReceivedMessages = receivedMessages
            };

            return this.View(userMessagesModel);
        }
    }
}
