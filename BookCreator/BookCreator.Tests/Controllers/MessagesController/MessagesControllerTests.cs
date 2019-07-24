using System;
using BookCreator.Services.Interfaces;
using BookCreator.Services.Utilities;
using BookCreator.ViewModels.InputModels.Messages;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using NUnit.Framework;

namespace BookCreator.Tests.Controllers.MessagesController
{
    [TestFixture]
    public class MessagesControllerTests
    {
        [Test]
        public void AddMessage_Return_Error_EmptyMessage()
        {
            var messageService = new Mock<IMessageService>();

            var message = new MessageInputModel
            {
                Message = null,
                ReceiverName = "usertest",
                SenderName = "admintest",
                SendDate = DateTime.UtcNow
            };

            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
            {
                [GlobalConstants.Error] = GlobalConstants.EmptyMessage
            };
            var controller = new BookCreatorApp.Controllers.MessagesController(messageService.Object, null, null)
            {
                TempData = tempData
            };

            var result = controller.SendMessage(message);

            string action = "Profile";
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(action);
        }

        [Test]
        public void Controller_Should_Contains_Authorize()
        {
            typeof(BookCreatorApp.Controllers.MessagesController).Should().BeDecoratedWith<AuthorizeAttribute>();
        }
    }
}
