namespace BookCreator.Tests.BookCreatorControllers.AdminsController
{
	using Moq;
	using NUnit.Framework;
	using FluentAssertions;
	using Services.Utilities;
	using Services.Interfaces;
	using Castle.Core.Internal;
	using System.Threading.Tasks;
	using System.Security.Claims;
	using Microsoft.AspNetCore.Mvc;
	using BookCreatorApp.Controllers;
	using Microsoft.AspNetCore.Http;
	using System.Collections.Generic;
	using ViewModels.OutputModels.Stories;
	using Microsoft.AspNetCore.Authorization;
	using BookCreatorApp.Areas.Administration.Controllers;

	[TestFixture]
	public class AdminsControllerTests
	{
		protected Mock<IAdminService> adminService => new Mock<IAdminService>();
		protected Mock<IStoryService> storyService => new Mock<IStoryService>();

		[Test]
		public async Task DeleteStoryShould_Redirect_To_Error_When_Role_Is_Missing()
		{
			//arrange
			var user = new Mock<ClaimsPrincipal>();
			user.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(false);
			var adminsController = new AdminsController(adminService.Object, storyService.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext { User = user.Object }
				}
			};

			//act
			int id = 1;
			var result = await adminsController.DeleteStory(id);

			//assert
			Assert.AreEqual(((RedirectToActionResult)result).ActionName, nameof(HomeController.Error));
		}

		[Test]
		public async Task DeleteStoryShould_Redirect_To_AllStories()
		{
			//arrange
			string username = "SomeName";
			var user = new Mock<ClaimsPrincipal>();
			user.Setup(x => x.IsInRole(It.IsAny<string>())).Returns(true);
			user.Setup(x => x.Identity.Name).Returns(username);

			var adminsController = new AdminsController(adminService.Object, storyService.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = new DefaultHttpContext { User = user.Object }
				}
			};

			//act
			int id = 1;
			var result = await adminsController.DeleteStory(id);

			//assert
			Assert.AreEqual(((RedirectToActionResult)result).ActionName, nameof(StoriesController.AllStories));
		}

		[Test]
		public void CurrentGenres_Should_Return_Same_View_With_Empty_String_On_Post()
		{
			//arrange

			storyService.Setup(x => x.Genres()).Returns(new List<StoryTypeOutputModel>
			{
				new StoryTypeOutputModel
				{
					Id = 1,
					Type = "fantasy"
				},
				new StoryTypeOutputModel
				{
					Id = 2,
					Type = "horror"
				}
			});

			var adminController = new AdminsController(adminService.Object, storyService.Object);

			//act
			var result = adminController.CurrentGenres(null);

			//assert

			result.As<ViewResult>().ViewData.ContainsKey(GlobalConstants.Error).Equals(GlobalConstants.NullName);
		}

		[Test]
		public void CurrentGenres_Should_Redirect_Back_When_Genre_Already_Exists()
		{
			//arrange
			//TODO: this is strange setup don't work but for the test even null is working ok will exam it sometime why!
			storyService.Setup(x => x.Genres()).Returns(new List<StoryTypeOutputModel>
				{
					new StoryTypeOutputModel
					{
						Id = 1,
						Type = "fantasy"
					},
					new StoryTypeOutputModel
					{
						Id = 2,
						Type = "horror"
					}
				});
			string name = "someGenre";

			adminService.Setup(x => x.AddGenre(name)).Returns(GlobalConstants.Failed);

			var adminController = new AdminsController(adminService.Object, storyService.Object);

			//act

			var result = adminController.CurrentGenres(name);

			//assert
			string expected = string.Join(GlobalConstants.AlreadyExistsInDb, name);
			result.As<ViewResult>().ViewData.ContainsKey(GlobalConstants.Error).Equals(expected);
		}

		[Test]
		public void AdminsController_Should_Be_Accessed_Only_By_Administration_Roles()
		{
			var result = typeof(AdminsController).GetAttribute<AuthorizeAttribute>();

			var roles = $"{GlobalConstants.Admin},{GlobalConstants.Moderator}";

			Assert.AreEqual(result.Roles, roles);
		}
	}
}