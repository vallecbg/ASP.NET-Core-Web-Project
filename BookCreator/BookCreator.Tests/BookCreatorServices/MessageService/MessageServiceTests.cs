namespace BookCreator.Tests.BookCreatorServices.MessageService
{
    using Base;
    using Models;
    using System;
    using System.Linq;
    using NUnit.Framework;
    using FluentAssertions;
    using Services.Interfaces;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Identity;
    using ViewModels.InputModels;
    using ViewModels.OutputModels.InfoHub;

    [TestFixture]
    public class MessageServiceTests : BaseServiceFake
    {
        private IMessageService messageService => (IMessageService)this.Provider.GetService(typeof(IMessageService));
        private UserManager<BookCreatorUser> userManager => (UserManager<BookCreatorUser>)this.Provider.GetService(typeof(UserManager<BookCreatorUser>));

        [Test]
        public void Infohub_Returning_Correct_Messsages_And_Notification_For_User()
        {
            //arrange
            var user = new BookCreatorUser
            {
                Id = "user",
                UserName = "UserInfoHub"
            };

            var messages = new[]
            {
                new Message
                {
                    IsReaden = true,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 1,
                    Text = "It works?",
                },

                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 2,
                    Text = "It works?two",
                }
            };

            var notifications = new[]
            {
                new Notification
                {
                    BookCreatorUser = user,
                    BookCreatorUserId = user.Id,
                    Message = "boom",
                    Seen = false,
                },
                new Notification
                {
                    BookCreatorUser = user,
                    BookCreatorUserId = user.Id,
                    Message = "boom",
                    Seen = false,
                }
            };

            this.Context.Messages.AddRange(messages);
            this.Context.Notifications.AddRange(notifications);
            this.userManager.CreateAsync(user).GetAwaiter().GetResult();

            this.Context.SaveChanges();

            //act
            string username = user.UserName;
            var result = this.messageService.Infohub(username);

            //assert

            result.Should().BeOfType<InfoHubViewModel>();
            result.Notifications.As<IEnumerable<NotificationOutputModel>>()
                .Should().NotBeEmpty().And.HaveCount(2)
                .And.ContainItemsAssignableTo<NotificationOutputModel>();

            result.OldMessages.As<IEnumerable<MessageOutputModel>>()
                .Should().NotBeEmpty().And.HaveCount(1)
                .And.ContainItemsAssignableTo<MessageOutputModel>();

            result.NewMessages.As<IEnumerable<MessageOutputModel>>()
                .Should().NotBeEmpty().And.HaveCount(1)
                .And.ContainItemsAssignableTo<MessageOutputModel>();
        }

        [Test]
        public void All_Message_Seen_Should_Change_For_All_Messages_IsReaden_To_True()
        {
            //arrange
            var user = new BookCreatorUser
            {
                Id = "user",
                UserName = "UserInfoHub"
            };

            var messages = new[]
            {
                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 1,
                    Text = "It works?",
                },

                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 2,
                    Text = "It works?two",
                }
            };

            this.Context.Messages.AddRange(messages);
            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            //act
            string username = user.UserName;
            this.messageService.AllMessagesSeen(username);

            //assert

            var messagesFromDb = this.Context.Messages.Where(x => x.ReceiverId == user.Id).ToArray();

            messagesFromDb
                .Select(
                    x => x.IsReaden.Should()
                        .NotBeFalse())
                .Should().HaveCount(2);
        }

        [Test]
        public void Delete_All_Messages_Should_Delete_All_Messages_For_User()
        {
            //arrange
            var user = new BookCreatorUser
            {
                Id = "user",
                UserName = "UserInfoHub"
            };

            var messages = new[]
            {
                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 1,
                    Text = "It works?",
                },

                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 2,
                    Text = "It works?two",
                }
            };

            this.Context.Messages.AddRange(messages);
            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            //act
            string userId = user.Id;
            this.messageService.DeleteAllMessages(userId);

            //assert
            var messagesFromDb = this.Context.Messages.Where(x => x.ReceiverId == user.Id).ToArray();

            messagesFromDb.Should().BeEmpty();
        }

        [Test]
        public void Delete_Message_Should_Delete_Only_The_Message_With_Given_Id()
        {
            var user = new BookCreatorUser
            {
                Id = "user",
                UserName = "UserInfoHub"
            };

            var messages = new[]
            {
                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 1,
                    Text = "It works?",
                },

                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 2,
                    Text = "It works?two",
                }
            };

            this.Context.Messages.AddRange(messages);
            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            //act
            int messageId = messages[0].Id;
            this.messageService.DeleteMessage(messageId);

            //assert
            var messagesFromDb = this.Context.Messages.Where(x => x.ReceiverId == user.Id).ToArray();
            int singleMessageIdShouldBe = 2;

            messagesFromDb.Should()
                .ContainSingle()
                .And.Subject.Should()
                .Contain(x => x.Id == singleMessageIdShouldBe);
        }

        [Test]
        public void NewMessages_Should_Return_The_Count_Of_Messages_Where_Is_Readen_Is_False()
        {
            //arrange
            var user = new BookCreatorUser
            {
                Id = "user",
                UserName = "UserInfoHub"
            };

            var messages = new[]
            {
                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 1,
                    Text = "It works?",
                },

                new Message
                {
                    IsReaden = false,
                    ReceiverId = user.Id,
                    Receiver = user,
                    Id = 2,
                    Text = "It works?two",
                }
            };

            this.Context.Messages.AddRange(messages);
            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            //act
            string userId = user.Id;
            int count = this.messageService.NewMessages(userId);
            int expectedCount = messages.Count();
            //assert
            count.Should().BePositive().And.Subject.Should().Be(expectedCount);
        }

        [Test]
        public void MessageSeen_Should_Change_Message_IsReaden_Property_To_True()
        {
            //arrange
            var user = new BookCreatorUser
            {
                Id = "user",
                UserName = "UserInfoHub"
            };

            var message = new Message
            {
                IsReaden = false,
                ReceiverId = user.Id,
                Receiver = user,
                Id = 1,
                Text = "It works?",
            };

            this.Context.Messages.Add(message);
            this.userManager.CreateAsync(user).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            //act
            int messageId = message.Id;
            this.messageService.MessageSeen(messageId);

            //assert
            var result = this.Context.Messages.Find(messageId);

            result.Should().NotBeNull()
                .And.Subject.As<Message>().IsReaden
                .Should().BeTrue();
        }

        [Test]
        public void Send_Message_Should_Create_New_Message()
        {
            //arrange
            var sender = new BookCreatorUser
            {
                Id = "sender",
                UserName = "UserSender"
            };
            var receiver = new BookCreatorUser
            {
                Id = "receiver",
                UserName = "UserReceiver"
            };

            var message = new MessageInputModel
            {
                SenderName = sender.UserName,
                ReceiverName = receiver.UserName,
                SendDate = DateTime.UtcNow,
                Message = "It works!!",
            };

            this.userManager.CreateAsync(receiver).GetAwaiter().GetResult();
            this.userManager.CreateAsync(sender).GetAwaiter().GetResult();
            this.Context.SaveChanges();

            //act
            this.messageService.SendMessage(message);

            //assert
            var result = this.Context.Messages.FirstOrDefault();

            result.Should().NotBeNull();
            result?.SenderId.Should().Be(sender.Id);
            result?.ReceiverId.Should().Be(receiver.Id);
        }

        [Test]
        public void CanSendMessage_Should_Return_True()
        {
            //arrange
            var sender = new BookCreatorUser
            {
                Id = "sender",
                UserName = "UserSender"
            };
            var receiver = new BookCreatorUser
            {
                Id = "receiver",
                UserName = "UserReceiver"
            };

            this.userManager.CreateAsync(receiver).GetAwaiter().GetResult();
            this.userManager.CreateAsync(sender).GetAwaiter().GetResult();
            this.Context.SaveChanges();
            //act
            string sendername = sender.UserName;
            string receivername = receiver.UserName;

            var possible = this.messageService.CanSendMessage(sendername, receivername);

            //assert

            possible.Should().BeTrue();
        }

        [Test]
        public void CanSendMessage_Should_Return_False()
        {
            //arrange
            var sender = new BookCreatorUser
            {
                Id = "sender",
                UserName = "UserSender"
            };
            var receiver = new BookCreatorUser
            {
                Id = "receiver",
                UserName = "UserReceiver"
            };

            var blocked = new BlockedUsers
            {
                BlockedUser = receiver,
                BlockedUserId = receiver.Id,
                BookCreatorUser = sender,
                FanfictionUserId = sender.Id
            };

            this.userManager.CreateAsync(receiver).GetAwaiter().GetResult();
            this.userManager.CreateAsync(sender).GetAwaiter().GetResult();
            this.Context.BlockedUsers.Add(blocked);
            this.Context.SaveChanges();
            //act
            string sendername = sender.UserName;
            string receivername = receiver.UserName;

            var possible = this.messageService.CanSendMessage(sendername, receivername);

            //assert

            possible.Should().BeFalse();
        }
    }
}