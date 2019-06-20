namespace BookCreator.Tests.BookCreatorControllers.MessagesController
{
	using Moq;
	using System;
	using NUnit.Framework;
	using FluentAssertions;
	using Services.Interfaces;
	using Services.Utilities;
	using ViewModels.InputModels;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Http;
	using BookCreatorApp.Controllers;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;

	[TestFixture]
	public class MessagesControllerTests
	{
		[Test]
		public void Controller_should_Have_Authorize_Attribute()
		{
			typeof(MessagesController).Should().BeDecoratedWith<AuthorizeAttribute>();
		}

		[Test]
		public void MessagesController_Should_Return_View_With_Error_If_Message_Content_Empty()
		{
			var messageService = new Mock<IMessageService>();

			var message = new MessageInputModel
			{
				Message = null,
				ReceiverName = "someName",
				SenderName = "anotherName",
				SendDate = DateTime.Now
			};

			var httpContext = new DefaultHttpContext();
			var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
			{
				[GlobalConstants.Error] = GlobalConstants.EmptyMessage
			};
			var controller = new MessagesController(messageService.Object)
			{
				TempData = tempData
			};

			var result = controller.SendMessage(message);

			string action = "Profile";
			result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be(action);
		}
	}
}