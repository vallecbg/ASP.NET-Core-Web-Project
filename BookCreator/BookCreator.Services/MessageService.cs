using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BookCreator.Data;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.ViewModels.InputModels.Messages;
using Microsoft.AspNetCore.Identity;

namespace BookCreator.Services
{
    public class MessageService : BaseService, IMessageService
    {
        public MessageService(UserManager<BookCreatorUser> userManager, BookCreatorContext context, IMapper mapper) : base(userManager, context, mapper)
        {
        }

        public void SendMessage(MessageInputModel inputModel)
        {
            var sender =  this.UserManager.FindByNameAsync(inputModel.SenderName).GetAwaiter().GetResult();
            var receiver =  this.UserManager.FindByNameAsync(inputModel.ReceiverName).GetAwaiter().GetResult();

            var message = new Message()
            {
                IsRead = false,
                Receiver = receiver,
                Sender = sender,
                SentOn = DateTime.UtcNow,
                Text = inputModel.Message
            };

            this.Context.Messages.Add(message);
            this.Context.SaveChanges();
        }
    }
}
