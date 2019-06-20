namespace BookCreator.Tests.BookCreatorServices.UserService
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
	using AutoMapper.QueryableExtensions;
	using Base;
	using FluentAssertions;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.DependencyInjection;
	using Models;
	using NUnit.Framework;
	using Services.Interfaces;
	using Services.Utilities;
	using ViewModels.InputModels;
	using ViewModels.OutputModels.Announcements;
	using ViewModels.OutputModels.InfoHub;
	using ViewModels.OutputModels.Stories;
	using ViewModels.OutputModels.Users;

	[TestFixture]
	public class UserServiceTests : BaseServiceFake
	{
		protected UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
		protected IUserService userService => this.Provider.GetRequiredService<IUserService>();
		protected RoleManager<IdentityRole> roleService => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

		[Test]
		public void LogUser_Should_Return_Sucess()
		{
			//arrange
			var user = new BookCreatorUser
			{
				UserName = "TestUser",
				Nickname = "TestLogin",
			};
			string userPassword = "123";

			this.userManager.CreateAsync(user).GetAwaiter().GetResult();
			this.userManager.AddPasswordAsync(user, userPassword).GetAwaiter().GetResult();
			this.Context.SaveChanges();

			//act

			var loginUser = new LoginInputModel
			{
				Password = userPassword,
				Nickname = user.Nickname
			};

			var result = this.userService.LogUser(loginUser);

			//assert
			result.Should().BeEquivalentTo(SignInResult.Success);
		}

		[Test]
		public void LogUser_Should_Return_Failed()
		{
			//arrange
			string nickname = "someNickname";
			string userPassword = "123";

			//act

			var loginUser = new LoginInputModel
			{
				Password = userPassword,
				Nickname = nickname
			};

			var result = this.userService.LogUser(loginUser);

			//assert
			result.Should().BeEquivalentTo(SignInResult.Failed);
		}

		[Test]
		public void RegisterUser_ShouldSucceed()
		{
			//arrange
			var toRegister = new RegisterInputModel
			{
				Nickname = "NewUser",
				Password = "123",
				ConfirmPassword = "123",
				Email = "some@mail.com",
				Username = "GetMeIn"
			};

			//act
			this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
			var result = this.userService.RegisterUser(toRegister).GetAwaiter().GetResult();

			//assert

			result.Should().BeEquivalentTo(SignInResult.Success);
		}

		[Test]
		public void RegisterUser_Should_Fail_With_Same_Nickname()
		{
			//arrange
			var toRegister = new RegisterInputModel
			{
				Nickname = "NewUser",
				Password = "123",
				ConfirmPassword = "123",
				Email = "some@mail.com",
				Username = "GetMeIn"
			};

			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.Context.SaveChanges();
			//act
			this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
			var result = this.userService.RegisterUser(toRegister).GetAwaiter().GetResult();

			//assert

			result.Should().BeEquivalentTo(SignInResult.Failed);
		}

		[Test]
		public void RegisterUser_Should_Fail_With_Same_Username()
		{
			//arrange
			var toRegister = new RegisterInputModel
			{
				Nickname = "NewUser",
				Password = "123",
				ConfirmPassword = "123",
				Email = "some@mail.com",
				Username = "GetMeIn"
			};

			var user = new BookCreatorUser
			{
				Nickname = "NewUserTwo",
				Email = "some@mail.com",
				UserName = "GetMeIn"
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.Context.SaveChanges();
			//act
			this.roleService.CreateAsync(new IdentityRole { Name = GlobalConstants.DefaultRole }).GetAwaiter();
			var result = this.userService.RegisterUser(toRegister).GetAwaiter().GetResult();

			//assert

			result.Should().BeEquivalentTo(SignInResult.Failed);
		}

		[Test]
		public void GetHomeViewDetails_Should_Return_Correct_IEnumumerables()
		{
			//arrange

			var storyType = new StoryType
			{
				Name = "fantasy"
			};

			var user = new BookCreatorUser
			{
				Nickname = "User",
				UserName = "someUser"
			};

			this.userManager.CreateAsync(user).GetAwaiter();

			var stories = new[]
			{
				new BookCreatorStory
				{
					Id=1,
					CreatedOn = DateTime.Now.AddHours(2),
					Type = storyType,
					Author = user,
					Summary = "someSummary1",
					Title = "SomeTitle1"
				},
				new BookCreatorStory
				{
					Id=2,
					CreatedOn = DateTime.Now,
					Type = storyType,
					Author = user,
					Summary = null,
					Title = "SomeTitle2"
				},
				new BookCreatorStory
				{
					Id=3,
					CreatedOn = DateTime.Now.AddDays(-1),
					Type = storyType,
					Author = user,
					Summary = "someSummary2",
					Title = "SomeTitle3"
				},
			};

			var announcements = new[]
			{
				new Announcement
				{
					Author = user,
					Content = "someContent1",
					Id = 1,
					PublshedOn = DateTime.Now
				},
				new Announcement
				{
					Author = user,
					Content = "someContent2",
					Id = 2,
					PublshedOn = DateTime.Now.AddHours(2)
				},
				new Announcement
				{
					Author = user,
					Content = "someContent3",
					Id = 3,
					PublshedOn = DateTime.Now.AddDays(-1)
				},
				new Announcement
				{
					Author = user,
					Content = "someContent4",
					Id = 4,
					PublshedOn = DateTime.Now.AddMonths(-1)
				},
			};

			this.Context.StoryTypes.Add(storyType);
			this.Context.FictionStories.AddRange(stories);
			this.Context.Announcements.AddRange(announcements);
			this.Context.SaveChanges();

			//act

			var result = this.userService.GetHomeViewDetails();

			//assert
			int anounceCount = 3;
			int storiesCount = 2;
			string summary = stories[2].Summary;
			var date = stories[2].CreatedOn;
			var title = stories[2].Title;
			string anounceContent = announcements[3].Content;
			var anounceDate = announcements[3].PublshedOn.ToShortDateString();
			var storyToNotBeIn = new StoryHomeOutputModel
			{
				Id = 3,
				Summary = summary,
				Author = user.UserName,
				CreatedOn = date,
				Rating = 0,
				StoryType = storyType.Name,
				Title = title
			};

			var notice = new AnnouncementOutputModel
			{
				Author = user.UserName,
				Content = anounceContent,
				PublishedOn = anounceDate
			};

			result.Stories.Should().NotBeNullOrEmpty()
				.And.HaveCount(storiesCount)
				.And.Subject.Should().NotContain(storyToNotBeIn);

			result.Announcements.Should().NotBeNullOrEmpty()
				.And.HaveCount(anounceCount)
				.And.Subject.Should().NotContain(notice);
		}

		[Test]
		public void BlockedUsers_Should_Return_Correct_Count()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			var userOne = new BookCreatorUser
			{
				Nickname = "NewUser2",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			var userTwo = new BookCreatorUser
			{
				Nickname = "NewUser3",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(userOne).GetAwaiter();
			this.userManager.CreateAsync(userTwo).GetAwaiter();

			var blockedPairs = new[]
			{
				new BlockedUsers
				{
					BookCreatorUser = user,
					BlockedUser = userOne
				},
				new BlockedUsers
				{
					BookCreatorUser = user,
					BlockedUser = userTwo
				},
			};
			this.Context.BlockedUsers.AddRange(blockedPairs);
			this.Context.SaveChanges();

			//act
			int count = blockedPairs.Length;
			string userId = user.Id;
			var result = this.userService.BlockedUsers(userId);

			//assert
			result.Should().NotBeNullOrEmpty()
				.And.HaveCount(count);
		}

		[Test]
		public void BlockUser_Should_Work_Correctly()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			var userOne = new BookCreatorUser
			{
				Nickname = "NewUser2",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(userOne).GetAwaiter();
			this.Context.SaveChanges();

			//act
			string currentUserName = user.UserName;
			string toBeBlocked = userOne.UserName;
			this.userService.BlockUser(currentUserName, toBeBlocked).GetAwaiter();

			//assert

			var blockedFromDb = new BlockedUsers
			{
				BookCreatorUser = user,
				BlockedUser = userOne,
				FanfictionUserId = user.Id,
				BlockedUserId = userOne.Id
			};

			var blocked = this.Context.BlockedUsers.FirstOrDefault();

			blocked.Should().NotBeNull()
				.And.Subject.Should().BeEquivalentTo(blockedFromDb);
		}

		[Test]
		public void BlockUser_Should_Throw_Exception()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			var userOne = new BookCreatorUser
			{
				Nickname = "NewUser2",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			var blocked = new BlockedUsers
			{
				BookCreatorUser = user,
				BlockedUser = userOne,
				FanfictionUserId = user.Id,
				BlockedUserId = userOne.Id
			};
			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(userOne).GetAwaiter();
			this.Context.BlockedUsers.Add(blocked);
			this.Context.SaveChanges();

			//act
			string currentUserName = user.UserName;
			string toBeBlocked = userOne.UserName;
			Func<Task> act = async () => await this.userService.BlockUser(currentUserName, toBeBlocked);

			//assert

			act.Should().Throw<InvalidOperationException>()
				.WithMessage(String.Format(GlobalConstants.AlreadyExistsInDb, typeof(BlockedUsers).Name));
		}

		[Test]
		public void UnBlockUser_Should_Work_Correctly()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			var userOne = new BookCreatorUser
			{
				Nickname = "NewUser2",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			var blocked = new BlockedUsers
			{
				BookCreatorUser = user,
				BlockedUser = userOne,
				FanfictionUserId = user.Id,
				BlockedUserId = userOne.Id
			};
			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(userOne).GetAwaiter();
			this.Context.BlockedUsers.Add(blocked);
			this.Context.SaveChanges();

			//act
			string userId = user.Id;
			string unblockId = userOne.Id;
			this.userService.UnblockUser(userId, unblockId);

			//assert
			var blockedFromDb = this.Context.BlockedUsers;
			blockedFromDb.Should().BeNullOrEmpty();
		}

		[Test]
		public void UnBlockUser_Should_Throw_Exception()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			var userOne = new BookCreatorUser
			{
				Nickname = "NewUser2",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(userOne).GetAwaiter();
			this.Context.SaveChanges();

			//act
			string userId = user.Id;
			string unblockId = userOne.Id;

			Action act = () => this.userService.UnblockUser(userId, unblockId);

			//assert

			act.Should().Throw<InvalidOperationException>()
				.WithMessage(GlobalConstants.NoRecordInDb);
		}

		[Test]
		public void GetUser_Should_Return_Correct_Model()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "NewUser",
				Email = "some@mail.com",
				UserName = "GetMeIn2"
			};

			var userOne = new BookCreatorUser
			{
				Nickname = "NewUser2",
				Email = "some@mail.com",
				UserName = "GetMeIn3"
			};

			var usertwo = new BookCreatorUser
			{
				Nickname = "User",
				UserName = "someUser"
			};

			var blockedOne = new BlockedUsers
			{
				FanfictionUserId = user.Id,
				BlockedUserId = userOne.Id
			};

			var blockedTwo = new BlockedUsers
			{
				BlockedUserId = user.Id,
				FanfictionUserId = usertwo.Id,
			};

			var storyType = new StoryType
			{
				Name = "fantasy"
			};

			var stories = new[]
			{
				new BookCreatorStory
				{
					Id=1,
					CreatedOn = DateTime.Now.AddHours(2),
					Type = storyType,
					AuthorId = user.Id,
					Summary = "someSummary1",
					Title = "SomeTitle1"
				},
				new BookCreatorStory
				{
					Id=2,
					CreatedOn = DateTime.Now,
					Type = storyType,
					AuthorId = user.Id,
					Summary = null,
					Title = "SomeTitle2"
				},
				new BookCreatorStory
				{
					Id=3,
					CreatedOn = DateTime.Now.AddDays(-1),
					Type = storyType,
					AuthorId = userOne.Id,
					Summary = "someSummary2",
					Title = "SomeTitle3"
				},
			};

			var comments = new[]
			{
				new Comment
				{
					CommentedOn = DateTime.Now,
					BookCreatorUser = user,
					BookCreatorStory = stories[0],
					Id = 1,
					Message = "some message"
				},
				new Comment
				{
					CommentedOn = DateTime.Now.AddHours(2),
					BookCreatorUser = user,
					BookCreatorStory = stories[1],
					Id = 2,
					Message = "some message2"
				},
			};

			var followed = new[]
			{
				new UserStory
				{
					FanfictionUserId = user.Id,
					BookCreatorStoryId = stories[2].Id
				},
				new UserStory
				{
					FanfictionUserId = usertwo.Id,
					BookCreatorStoryId = stories[1].Id
				},
			};

			var notice = new Notification
			{
				BookCreatorUser = user,
				Seen = false,
				Message = "madmamda",
				UpdatedStoryId = 3
			};

			var messages = new[]
			{
				new Message
				{
					SenderId = usertwo.Id,
					IsReaden = true,
					Text = "dadsdadsasa",
					ReceiverId = user.Id,
					Id = 1,
					SendOn = DateTime.Now.AddHours(2)
				},
				new Message
				{
					SenderId = user.Id,
					IsReaden = false,
					Text = "dadsa",
					ReceiverId = userOne.Id,
					Id = 2,
					SendOn = DateTime.Now
				},
			};

			this.Context.Notifications.Add(notice);
			this.Context.UsersStories.AddRange(followed);
			this.Context.Messages.AddRange(messages);
			this.Context.StoryTypes.Add(storyType);
			this.Context.FictionStories.AddRange(stories);
			this.Context.Comments.AddRange(comments);
			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(userOne).GetAwaiter();
			this.userManager.CreateAsync(usertwo).GetAwaiter();
			this.Context.BlockedUsers.AddRange(blockedOne);
			this.Context.BlockedUsers.AddRange(blockedTwo);
			this.Context.SaveChanges();

			//act
			string username = user.UserName;
			var model = this.userService.GetUser(username);

			var notifications = this.Context.Notifications
				.Where(x => x.BookCreatorUserId == user.Id)
				.ProjectTo<NotificationOutputModel>().ToList();
			var messagesUser = this.Context.Messages.Where(x => x.ReceiverId == user.Id || x.SenderId == user.Id)
				.ProjectTo<MessageOutputModel>()
				.ToList();
			var followedStories = this.Context.UsersStories.Where(x => x.FanfictionUserId == user.Id)
				.Select(x => x.BookCreatorStory)
				.ProjectTo<StoryOutputModel>().ToList();

			var userStories = this.Context.FictionStories.Where(x => x.AuthorId == user.Id)
				.ProjectTo<StoryOutputModel>().ToList();
			var modelToCompare = new UserOutputViewModel
			{
				BlockedUsers = 1,
				BlockedBy = 1,
				Comments = 2,
				Email = user.Email,
				FollowedStories = followedStories,
				Id = user.Id,
				Username = user.UserName,
				NickName = user.Nickname,
				Messages = messagesUser,
				MessagesCount = messagesUser.Count,
				Notifications = notifications,
				Role = GlobalConstants.DefaultRole,
				UserStories = userStories,
				Stories = userStories.Count,
			};

			model.Should().NotBeNull().And.Subject.Should().Equals(modelToCompare);
		}
	}
}