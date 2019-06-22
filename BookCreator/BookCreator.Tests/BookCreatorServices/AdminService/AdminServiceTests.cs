namespace BookCreator.Tests.BookCreatorServices.AdminService
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;
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
	using ViewModels.OutputModels.Users;

	[TestFixture]
	public class AdminServiceTests : BaseServiceFake
	{
		private UserManager<BookCreatorUser> userManager => this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
		private IAdminService adminService => this.Provider.GetRequiredService<IAdminService>();
		private RoleManager<IdentityRole> roleManager => this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

		[Test]
		public void AllUsers_Should_Return_Correct_Info_Per_User()
		{
			//arrange
			var author = new BookCreatorUser
			{
				Id = "AuthorId",
				Nickname = "StoryAuthor",
				UserName = "Author",
			};

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};

			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test",
				AuthorId = author.Id,
			};

			var comments = new[]
			{
				new Comment
				{
					BookCreatorUser=author,
					Id=1,
					UserId=author.Id,
					StoryId=story.Id,
					BookCreatorStory=story,
					Message="someMessage"
				},
				new Comment
				{
					BookCreatorUser=author,
					Id=2,
					UserId=author.Id,
					StoryId=story.Id,
					BookCreatorStory=story,
					Message="someMessageTwo"
				},
			};

			var messages = new[]
			{
				new Message
				{
					Id=1,
					IsReaden=false,
					Receiver=author,
					ReceiverId=author.Id,
					Sender=user,
					SenderId=user.Id
				},
				new Message
				{
					Id=2,
					IsReaden=true,
					Receiver=author,
					ReceiverId=author.Id,
					Sender=user,
					SenderId=user.Id
				},
				new Message
				{
					Id=3,
					IsReaden=false,
					Receiver=user,
					ReceiverId=user.Id,
					Sender=author,
					SenderId=author.Id
				},
				new Message
				{
					Id=4,
					IsReaden=true,
					Receiver=user,
					ReceiverId=user.Id,
					Sender=author,
					SenderId=author.Id
				}
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(author).GetAwaiter();
			this.Context.FictionStories.Add(story);
			this.Context.Comments.AddRange(comments);
			this.Context.Messages.AddRange(messages);
			this.Context.SaveChanges();

			//act
			var result = this.adminService.AllUsers().GetAwaiter().GetResult();

			int indexToTakeFrom = 0;
			var userToCompare = result?.ElementAt(indexToTakeFrom);
			//assert
			int totalAuthorStories = 1;

			var expectedAuthorOutput = new UserAdminViewOutputModel
			{
				Id = author.Id,
				Comments = comments.Length,
				Nickname = author.Nickname,
				MessageCount = messages.Length,
				Username = author.UserName,
				Stories = totalAuthorStories,
				Role = GlobalConstants.DefaultRole
			};

			userToCompare.Should().NotBeNull()
		   .And.Subject.Should().BeEquivalentTo(expectedAuthorOutput);
		}

		[Test]
		public void DeleteUser_Should_Delete_User_By_Given_Id()
		{
			//arrange
			var author = new BookCreatorUser
			{
				Id = "AuthorId",
				Nickname = "StoryAuthor",
				UserName = "Author",
			};

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};
			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(author).GetAwaiter();
			this.Context.SaveChanges();

			//act
			string userToDeleteId = author.Id;
			this.adminService.DeleteUser(userToDeleteId);

			//assert
			var users = this.Context.Users.ToList();

			users.Should().ContainSingle()
				.And.Subject.Should().NotContain(author);
		}

		[Test]
		public void DeleteUser_Should_Throw_Argument_Exception()
		{
			//arrange
			var author = new BookCreatorUser
			{
				Id = "AuthorId",
				Nickname = "StoryAuthor",
				UserName = "Author",
			};

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};
			this.userManager.CreateAsync(user).GetAwaiter();
			this.userManager.CreateAsync(author).GetAwaiter();
			this.Context.SaveChanges();

			//act
			string userToDeleteId = "randomId";
			Func<Task> act = async () => await this.adminService.DeleteUser(userToDeleteId);

			//assert
			act.Should().Throw<ArgumentException>().WithMessage(GlobalConstants.ErrorOnDeleteUser);
		}

		[Test]
		public void DeleteAnnouncement_Should_Delete_Announcement_With_Given_Id()
		{
			//arrange

			var announcements = new[]
			{
				new Announcement
				{
					Id=1,
					Content="some content"
				},
				new Announcement
				{
					Id=2,
					Content="content"
				}
			};

			this.Context.Announcements.AddRange(announcements);
			this.Context.SaveChanges();

			//act
			int anounceId = announcements[0].Id;
			this.adminService.DeleteAnnouncement(anounceId);

			//assert
			var announces = this.Context.Announcements.ToList();
			var equalFromDb = announcements[1];
			announces.Should().NotBeEmpty()
				.And.Subject.Should()
				.ContainSingle()
				.And.Subject.Take(1)
				.Should().BeEquivalentTo(equalFromDb);
		}

		[Test]
		public void DeleteAllAnnouncements_Should_Delete_All_Announcements_In_Db()
		{
			//arrange

			var announcements = new[]
			{
				new Announcement
				{
					Id=1,
					Content="some content"
				},
				new Announcement
				{
					Id=2,
					Content="content"
				}
			};

			this.Context.Announcements.AddRange(announcements);
			this.Context.SaveChanges();

			//act
			this.adminService.DeleteAllAnnouncements();

			//assert
			var announces = this.Context.Announcements.ToList();
			announces.Should().BeEmpty();
		}

		[Test]
		public void Add_Genre_Should_Add_New_Genre()
		{
			//arrange

			var genres = new[]
			{
				new StoryType
				{
					Id = 2,
					Name = "fantasy"
				},
				new StoryType
				{
					Id = 3,
					Name = "horror"
				}
			};
			this.Context.StoryTypes.AddRange(genres);
			this.Context.SaveChanges();

			//act

			string newGenre = "Science Fiction";
			this.adminService.AddGenre(newGenre);

			//assert
			int expectedCount = 3;

			var existingGenres = this.Context.StoryTypes.ToList();
			existingGenres.Should().NotBeEmpty()
				.And.HaveCount(expectedCount)
				.And.Subject.Select(x => x.Name)
				.Should().Contain(newGenre);
		}

		[Test]
		public void Add_genre_Should_Fail_If_The_New_Genre_Already_Exists()
		{
			//arrange

			var genres = new[]
			{
				new StoryType
				{
					Id = 2,
					Name = "fantasy"
				},
				new StoryType
				{
					Id = 3,
					Name = "horror"
				}
			};
			this.Context.StoryTypes.AddRange(genres);
			this.Context.SaveChanges();

			//act

			string newGenre = "fantasy";
			string result = this.adminService.AddGenre(newGenre);

			//assert
			result.Should().NotBeNull().And.Subject.Should().Equals(GlobalConstants.Failed);
		}

		[Test]
		public void ChangeRole_Should_Fail_With_Non_Existant_Role()
		{
			//arrange
			var roles = new[]
			{
				"admin",
				"superUser",
				"moderator",
				"user"
			};

			foreach (var currentRolename in roles)
			{
				var role = new IdentityRole
				{
					Name = currentRolename
				};
				this.roleManager.CreateAsync(role).GetAwaiter();
			}

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.Context.SaveChanges();

			//act
			string newRole = "someRole";
			var model = new ChangingRoleModel
			{
				Id = user.Id,
				AppRoles = roles,
				NewRole = newRole,
				Nickname = user.Nickname,
				Role = GlobalConstants.DefaultRole
			};
			var methodResult = this.adminService.ChangeRole(model);

			//assert
			methodResult.Should().Equals(IdentityResult.Failed());
		}

		[Test]
		public void ChangeRole_Succeed_Role()
		{
			//arrange
			var roles = new[]
			{
				"admin",
				"superUser",
				"moderator",
				"user"
			};

			foreach (var currentRolename in roles)
			{
				var role = new IdentityRole
				{
					Name = currentRolename
				};
				this.roleManager.CreateAsync(role).GetAwaiter();
			}

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.Context.SaveChanges();

			//act
			string newRole = roles[2];
			var model = new ChangingRoleModel
			{
				Id = user.Id,
				AppRoles = roles,
				NewRole = newRole,
				Nickname = user.Nickname,
				Role = GlobalConstants.DefaultRole
			};
			var methodResult = this.adminService.ChangeRole(model);

			//assert
			methodResult.Should().Equals(IdentityResult.Failed());
		}

		[Test]
		public void AllAnnouncements_Should_Return_Correct_Number_Of_Announcements()
		{
			//arrange

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};

			var announcements = new[]
			{
				new Announcement
				{
					Author = user,
					AuthorId = user.Id,
					Content = "SomeAnnouncement1",
					Id=1,
					PublshedOn = DateTime.Now
				},
				new Announcement
				{
					Author = user,
					AuthorId = user.Id,
					Content = "SomeAnnouncement2",
					Id=2,
					PublshedOn = DateTime.Now
				},
				new Announcement
				{
					Author = user,
					AuthorId = user.Id,
					Content = "SomeAnnouncement3",
					Id=3,
					PublshedOn = DateTime.Now
				},
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.Context.Announcements.AddRange(announcements);
			this.Context.SaveChanges();

			//act
			var result = this.adminService.AllAnnouncements();

			//assert
			int count = announcements.Length;
			result.Announcements.Should()
				.NotBeEmpty()
				.And.HaveCount(count)
				.And.ContainItemsAssignableTo<AnnouncementOutputModel>();
		}

		[Test]
		public void Add_Announcement_Should_Add_Announcement()
		{
			//arrange

			var user = new BookCreatorUser
			{
				Id = "userId",
				Nickname = "AnotherUser",
				UserName = "user",
			};

			var announcements = new[]
			{
				new Announcement
				{
					Author = user,
					AuthorId = user.Id,
					Content = "SomeAnnouncement1",
					Id=4,
					PublshedOn = DateTime.Now
				},
				new Announcement
				{
					Author = user,
					AuthorId = user.Id,
					Content = "SomeAnnouncement2",
					Id=2,
					PublshedOn = DateTime.Now
				},
				new Announcement
				{
					Author = user,
					AuthorId = user.Id,
					Content = "SomeAnnouncement3",
					Id=3,
					PublshedOn = DateTime.Now
				},
			};

			this.userManager.CreateAsync(user).GetAwaiter();
			this.Context.Announcements.AddRange(announcements);
			this.Context.SaveChanges();

			//act

			var newAnounce = new AnnouncementInputModel
			{
				Author = user.UserName,
				Content = "NewAnnouncement"
			};
			this.adminService.AddAnnouncement(newAnounce);

			var modelCompare = new Announcement
			{
				Author = user,
				AuthorId = user.Id,
				Content = newAnounce.Content,
				Id = 1,
				PublshedOn = DateTime.Now
			};
			//assert
			// again id to 1 because by default inmemory dont keep reference and add from 1!
			int id = 1;
			var anounce = this.Context.Announcements.Find(id);

			anounce.Should().NotBeNull().And.BeEquivalentTo(modelCompare, options => options.Excluding(x => x.PublshedOn));
		}
	}
}