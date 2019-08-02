using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using BookCreator.Models;
using BookCreator.Services.Interfaces;
using BookCreator.Tests.Services.Base;
using BookCreator.ViewModels.InputModels.Messages;
using BookCreator.ViewModels.OutputModels.Messages;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BookCreator.Tests.Services.Message_Service
{
    [TestFixture]
    public class MessageServiceTests : BaseServiceFake
    {
        protected IMessageService messageService => this.Provider.GetRequiredService<IMessageService>();
        protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();

        [Test]
        public void SendMessage_Should_Success()
        {
            var sender = new BookCreatorUser()
            {
                UserName = "sender",
                Name = "Gosho Petkov"
            };

            var receiver = new BookCreatorUser()
            {
                UserName = "receiver",
                Name = "Petko Goshov"
            };

            this.userManager.CreateAsync(sender).GetAwaiter();
            this.userManager.CreateAsync(receiver).GetAwaiter();
            this.Context.SaveChanges();

            var messageInput = new MessageInputModel
            {
                SenderName = sender.UserName,
                ReceiverName = receiver.UserName,
                Message = "ko staa we",
                SendDate = DateTime.UtcNow
            };

            var newMessageId = this.messageService.SendMessage(messageInput);

            var result = this.Context.Messages.First();

            result.Should().NotBeNull()
                .And.Subject.As<Message>()
                .Id.Should().Be(newMessageId);
        }

        [Test]
        public void MarkMessageAsSeen_Should_Success()
        {
            var sender = new BookCreatorUser()
            {
                UserName = "sender",
                Name = "Gosho Petkov"
            };

            var receiver = new BookCreatorUser()
            {
                UserName = "receiver",
                Name = "Petko Goshov"
            };

            this.userManager.CreateAsync(sender).GetAwaiter();
            this.userManager.CreateAsync(receiver).GetAwaiter();
            this.Context.SaveChanges();

            var message = new Message()
            {
                Id = "1",
                IsRead = false,
                ReceiverId = receiver.Id,
                SenderId = sender.Id,
                SentOn = DateTime.UtcNow,
                Text = "asdasd"
            };

            this.Context.Messages.Add(message);
            this.Context.SaveChanges();

            this.messageService.MarkMessageAsSeen(message.Id);

            var result = this.Context.Messages.Find(message.Id);

            result.Should().NotBeNull()
                .And.Subject.As<Message>()
                .IsRead.Equals(true);
        }

        [Test]
        public void DeleteMessage_Should_Success()
        {
            var sender = new BookCreatorUser()
            {
                UserName = "sender",
                Name = "Gosho Petkov"
            };

            var receiver = new BookCreatorUser()
            {
                UserName = "receiver",
                Name = "Petko Goshov"
            };

            this.userManager.CreateAsync(sender).GetAwaiter();
            this.userManager.CreateAsync(receiver).GetAwaiter();
            this.Context.SaveChanges();

            var message = new Message()
            {
                Id = "1",
                IsRead = false,
                ReceiverId = receiver.Id,
                SenderId = sender.Id,
                SentOn = DateTime.UtcNow,
                Text = "asdasd"
            };

            this.Context.Messages.Add(message);
            this.Context.SaveChanges();

            this.messageService.DeleteMessage(message.Id);

            var result = this.Context.Messages.ToList();

            result.Should().BeEmpty();
        }

        [Test]
        public void GetSentMessages_Should_Return_Success()
        {
            var sender = new BookCreatorUser()
            {
                UserName = "sender",
                Name = "Gosho Petkov"
            };

            var receiver = new BookCreatorUser()
            {
                UserName = "receiver",
                Name = "Petko Goshov"
            };

            this.userManager.CreateAsync(sender).GetAwaiter();
            this.userManager.CreateAsync(receiver).GetAwaiter();
            this.Context.SaveChanges();

            var messages = new[]
            {
                new Message()
                {
                    Id = "1",
                    IsRead = false,
                    ReceiverId = receiver.Id,
                    SenderId = sender.Id,
                    SentOn = DateTime.UtcNow,
                    Text = "asdasd"
                },
                new Message()
                {
                    Id = "2",
                    IsRead = false,
                    ReceiverId = receiver.Id,
                    SenderId = sender.Id,
                    SentOn = DateTime.UtcNow,
                    Text = "qweqwewqe"
                },
                new Message()
                {
                    Id = "1",
                    IsRead = false,
                    ReceiverId = sender.Id,
                    SenderId = receiver.Id,
                    SentOn = DateTime.UtcNow,
                    Text = "asdasd"
                },
            };

            var result = this.messageService.GetSentMessages(sender.Id);

            var mappedMessages = Mapper.Map<IList<MessageOutputModel>>(messages);

            var expectedResult = mappedMessages.Take(2).ToList();

            result.Should().NotBeNull()
                .And.Subject.Equals(expectedResult);
            result.Should().Subject.Count().Equals(messages.Length - 1);
        }

        [Test]
        public void GetReceivedMessages_Should_Return_Success()
        {
            var sender = new BookCreatorUser()
            {
                UserName = "sender",
                Name = "Gosho Petkov"
            };

            var receiver = new BookCreatorUser()
            {
                UserName = "receiver",
                Name = "Petko Goshov"
            };

            this.userManager.CreateAsync(sender).GetAwaiter();
            this.userManager.CreateAsync(receiver).GetAwaiter();
            this.Context.SaveChanges();

            var messages = new[]
            {
                new Message()
                {
                    Id = "1",
                    IsRead = false,
                    ReceiverId = receiver.Id,
                    SenderId = sender.Id,
                    SentOn = DateTime.UtcNow,
                    Text = "asdasd"
                },
                new Message()
                {
                    Id = "2",
                    IsRead = false,
                    ReceiverId = receiver.Id,
                    SenderId = sender.Id,
                    SentOn = DateTime.UtcNow,
                    Text = "qweqwewqe"
                },
                new Message()
                {
                    Id = "1",
                    IsRead = false,
                    ReceiverId = sender.Id,
                    SenderId = receiver.Id,
                    SentOn = DateTime.UtcNow,
                    Text = "asdasd"
                },
            };

            var result = this.messageService.GetReceivedMessages(receiver.Id);

            var mappedMessages = Mapper.Map<IList<MessageOutputModel>>(messages);

            var expectedResult = mappedMessages.Take(2).ToList();

            result.Should().NotBeNull()
                .And.Subject.Equals(expectedResult);
            result.Should().Subject.Count().Equals(messages.Length - 1);
        }
    }
}
