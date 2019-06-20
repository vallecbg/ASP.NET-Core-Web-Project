namespace BookCreator.Tests.BookCreatorServices.StoryService
{
	using Base;
	using Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using FluentAssertions;
	using Services.Utilities;
	using Services.Interfaces;
	using ViewModels.InputModels;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using ViewModels.OutputModels.Stories;
	using Microsoft.Extensions.DependencyInjection;

	[TestFixture]
	public class StoryServiceTests : BaseServiceFake
	{
		//TODO: I am stupid should have made a private property for StoryService and inject it only once?!Should try it in other tests and refactor these after this!
		[Test]
		public void CurrentStories_with_Type_Null_Should_Return_All_Stories()
		{
			//arrange
			var stories = new BookCreatorStory[]
			{
				new BookCreatorStory
				{
					Title="One",
					Id=1,
					CreatedOn=DateTime.Now,
					Summary=null,
					ImageUrl=GlobalConstants.DefaultNoImage,
					Type=new StoryType
					{
						Id=1,
						Name="Fantasy"
					},
					AuthorId="1111"
				},
				new BookCreatorStory
				{
					Title="Two",
					Id=2,
					CreatedOn=DateTime.Now.AddMonths(1),
					Summary=null,
					ImageUrl=GlobalConstants.DefaultNoImage,
					Type=new StoryType
					{
						Id=2,
						Name="Horror"
					},
					AuthorId="2222"
				},
				new BookCreatorStory
				{
					Title="three",
					Id=3,
					CreatedOn=DateTime.Now.AddDays(2),
					Summary=null,
					ImageUrl=GlobalConstants.DefaultNoImage,
					Type=new StoryType
					{
						Id=3,
						Name="Science Fiction"
					},
					AuthorId="3333"
				},
			};
			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act

			var storyServices = GetService();
			var result = storyServices.CurrentStories(null);

			//assert

			result.Should().NotBeEmpty()
				.And.HaveCount(3);
		}

		[Test]
		public void CurrentStories_with_Type_Null_Should_Return_OnlyOneType_Stories()
		{
			//arrange
			var stories = new BookCreatorStory[]
			{
				new BookCreatorStory
				{
					Title="One",
					Id=1,
					CreatedOn=DateTime.Now,
					Summary=null,
					ImageUrl=GlobalConstants.DefaultNoImage,
					Type=new StoryType
					{
						Id=1,
						Name="Fantasy"
					},
					AuthorId="1111"
				},
				new BookCreatorStory
				{
					Title="Two",
					Id=2,
					CreatedOn=DateTime.Now.AddMonths(1),
					Summary=null,
					ImageUrl=GlobalConstants.DefaultNoImage,
					Type=new StoryType
					{
						Id=2,
						Name="Horror"
					},
					AuthorId="2222"
				},
				new BookCreatorStory
				{
					Title="three",
					Id=3,
					CreatedOn=DateTime.Now.AddDays(2),
					Summary=null,
					ImageUrl=GlobalConstants.DefaultNoImage,
					Type=new StoryType
					{
						Id=3,
						Name="Science Fiction"
					},
					AuthorId="3333"
				},
			};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act
			string storyType = "Fantasy";
			var storyService = GetService();
			var result = storyService.CurrentStories(storyType);

			//assert
			result.Should().NotBeEmpty()
				.And.ContainSingle(x => x.Type.Type == "Fantasy");
		}

		[Test]
		public void GenresGetCorrectlyTakenFromDb()
		{
			//arrange
			var genres = new StoryType[]
			{
				new StoryType
				{
					Id=1,
					Name = "Fantasy"
				},
				new StoryType
				{
					Id=2,
					Name="Young Adult"
				},
				new StoryType
				{
					Id=3,
					Name="Comedy"
				}
			};

			this.Context.StoryTypes.AddRange(genres);
			this.Context.SaveChanges();

			//act

			var storyService = GetService();

			var genresFromDb = storyService.Genres();

			genresFromDb.Should().HaveCount(3);
		}

		[Test]
		public void GetStoryById_Throw_Exception_with_NonExistant_Id()
		{
			//arrange

			var story = new BookCreatorStory
			{
				Title = "One",
				Id = 1,
				CreatedOn = DateTime.Now,
				Summary = null,
				ImageUrl = GlobalConstants.DefaultNoImage,
				Type = new StoryType
				{
					Id = 1,
					Name = "Fantasy"
				},
				AuthorId = "1111"
			};

			this.Context.FictionStories.Add(story);
			this.Context.SaveChanges();

			//act
			var storyService = GetService();
			Action act = () => storyService.GetStoryById(2);

			act.Should().Throw<ArgumentException>().WithMessage(GlobalConstants.MissingStory);
		}

		[Test]
		public void GetStoryById_Should_Return_StoryDetailsModel_With_Correct_Id()
		{
			//arrange

			var story = new BookCreatorStory
			{
				Title = "One",
				Id = 1,
				CreatedOn = DateTime.Now,
				Summary = null,
				ImageUrl = GlobalConstants.DefaultNoImage,
				Type = new StoryType
				{
					Id = 1,
					Name = "Fantasy"
				},
				AuthorId = "1111"
			};

			this.Context.FictionStories.Add(story);
			this.Context.SaveChanges();

			//act
			var storyService = GetService();

			var result = storyService.GetStoryById(1);

			result.Should().BeOfType<StoryDetailsOutputModel>().And.Should().NotBeNull();
		}

		[Test]
		public async Task Follow_Should_Create_UserStory_Entity_In_Db()
		{
			//arrange

			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test"
			};

			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};
			this.Context.FictionStories.Add(story);

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			await usermanager.CreateAsync(user);

			this.Context.SaveChanges();

			//act
			var storyService = GetService();

			var storyId = story.Id;
			var username = user.UserName;
			var userId = user.Id;
			await storyService.Follow(username, userId, storyId);

			//assert

			var result = this.Context.UsersStories.FirstOrDefault();

			var userStory = new UserStory
			{
				FanfictionUserId = user.Id,
				BookCreatorStoryId = story.Id
			};

			result.Should().BeOfType<UserStory>().And.Subject.Should().Equals(userStory);
		}

		[Test]
		public async Task Follow_Should_Throw_OperationException_When_Try_To_Add_Duplicate_Entity()
		{
			//arrange

			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test"
			};

			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};
			this.Context.FictionStories.Add(story);

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			await usermanager.CreateAsync(user);

			this.Context.SaveChanges();

			//act
			var storyService = GetService();

			var storyId = story.Id;
			var username = user.UserName;
			var userId = user.Id;
			await storyService.Follow(username, userId, storyId);

			//assert

			Action act = () => storyService.Follow(username, userId, storyId).GetAwaiter().GetResult();

			string message = string.Join(GlobalConstants.UserFollowAlready, user.UserName);
			act.Should().Throw<InvalidOperationException>().WithMessage(message);
		}

		[Test]
		public async Task UnFollow_Should_Remove_UserStoryLink_From_Db()
		{
			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test"
			};

			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};

			var userStory = new UserStory
			{
				FanfictionUserId = user.Id,
				BookCreatorStoryId = story.Id
			};

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			await usermanager.CreateAsync(user);

			this.Context.FictionStories.Add(story);
			this.Context.UsersStories.Add(userStory);
			this.Context.SaveChanges();
			this.Context.Entry(userStory).State = EntityState.Detached;

			//act
			var storyService = GetService();

			var storyId = story.Id;
			var userId = user.Id;

			storyService.UnFollow(userId, storyId).GetAwaiter().GetResult();

			//assert

			var result = this.Context.UsersStories.FirstOrDefault();

			result.Should().BeNull();
		}

		[Test]
		public async Task UnFollow_Should_Throw_Exception_For_Missing_UserStory_Entity()
		{
			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test"
			};

			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			await usermanager.CreateAsync(user);

			this.Context.FictionStories.Add(story);

			this.Context.SaveChanges();

			//act
			var storyService = GetService();

			var storyId = story.Id;
			var userId = user.Id;

			Func<Task> act = async () => await storyService.UnFollow(userId, storyId);

			//assert

			await act.Should().ThrowAsync<ArgumentNullException>();
		}

		[Test]
		public async Task Add_Rating_Should_Add_Rating_To_A_Story()
		{
			//arrange

			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test"
			};

			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			await usermanager.CreateAsync(user);
			this.Context.FictionStories.Add(story);
			this.Context.SaveChanges();
			//act

			var storyService = GetService();
			storyService.AddRating(story.Id, 10, user.UserName);

			//assert

			var storyRating = new BookCreatorRating
			{
				BookCreatorId = 1,
				RatingId = 1
			};

			var result = this.Context.FictionRatings.FirstOrDefault();

			result.Should().NotBeNull().And.
				Subject.Should()
				.BeOfType<BookCreatorRating>().And.Should()
				.BeEquivalentTo(storyRating, opt => opt.ExcludingMissingMembers());
		}

		[Test]
		public void Add_Rating_Should_Throw_Exception_If_AlreadyRated_Is_True()
		{
			//arrange
			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test"
			};

			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};

			var rating = new StoryRating
			{
				Rating = 8,
				UserId = user.Id,
				Id = 1,
				BookCreatorUser = user
			};

			var storyRating = new BookCreatorRating
			{
				BookCreatorId = story.Id,
				RatingId = rating.Id,
				BookCreatorStory = story,
				StoryRating = rating
			};

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			usermanager.CreateAsync(user).GetAwaiter();

			this.Context.FictionStories.Add(story);
			this.Context.StoryRatings.Add(rating);
			this.Context.FictionRatings.Add(storyRating);
			this.Context.SaveChanges();

			//act

			var storyService = GetService();

			Action act = () => storyService.AddRating(story.Id, 1, user.UserName);

			//assert
			act.Should().Throw<InvalidOperationException>().WithMessage(GlobalConstants.AlreadyRated);
		}

		[Test]
		public void DeleteStory_Should_Be_Successful_For_Admin()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};
			var author = new BookCreatorUser
			{
				Id = "AuthorId",
				Nickname = "StoryAuthor",
				UserName = "Author",
			};

			var story = new BookCreatorStory
			{
				Id = 1,
				Author = null,
				Summary = "some summary",
				Title = "Story To test",
				AuthorId = author.Id,
			};

			var role = new IdentityRole { Name = "admin" };

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			var roleManager = this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

			usermanager.CreateAsync(author).GetAwaiter();
			usermanager.CreateAsync(user).GetAwaiter();
			roleManager.CreateAsync(role).GetAwaiter();

			usermanager.AddToRoleAsync(user, "admin").GetAwaiter();

			this.Context.FictionStories.Add(story);
			this.Context.SaveChanges();

			//act
			var storyService = GetService();
			storyService.DeleteStory(story.Id, user.UserName).GetAwaiter();

			//assert

			var result = this.Context.FictionStories.FirstOrDefault();

			result.Should().BeNull();
		}

		[Test]
		public void DeleteStory_Should_Throw_Exception_With_Admin_And_Author_Bools_False()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};

			var someUser = new BookCreatorUser
			{
				Id = "AnotherUserId",
				Nickname = "ThirdUser",
				UserName = "AnotherUser",
			};

			var story = new BookCreatorStory
			{
				Id = 1,
				Author = someUser,
				Summary = "some summary",
				Title = "Story To test",
				AuthorId = "someUserId",
			};

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			usermanager.CreateAsync(user).GetAwaiter();
			usermanager.CreateAsync(someUser).GetAwaiter();

			this.Context.FictionStories.Add(story);
			this.Context.SaveChanges();

			//act
			var storyService = GetService();
			Func<Task> act = async () => await storyService.DeleteStory(story.Id, user.UserName);

			//assert

			act.Should().Throw<OperationCanceledException>();
		}

		[Test]
		public void DeleteStory_Should_Throw_Exception_For_Null_Not_Existing_Story_On_Delete()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Id = "UserId",
				Nickname = "TestStory",
				UserName = "WhatEver",
			};
			var author = new BookCreatorUser
			{
				Id = "AuthorId",
				Nickname = "StoryAuthor",
				UserName = "Author",
			};

			var role = new IdentityRole { Name = "admin" };

			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			var roleManager = this.Provider.GetRequiredService<RoleManager<IdentityRole>>();

			usermanager.CreateAsync(author).GetAwaiter();
			usermanager.CreateAsync(user).GetAwaiter();
			roleManager.CreateAsync(role).GetAwaiter();

			usermanager.AddToRoleAsync(user, "admin").GetAwaiter();
			this.Context.SaveChanges();

			//act
			var storyService = GetService();
			Func<Task> act = async () => await storyService.DeleteStory(1, user.UserName);

			//assert

			act.Should().ThrowAsync<InvalidOperationException>().Wait();
		}

		[Test]
		public void CreateStory_Should_Work_Correct_With_Null_Iform_File()
		{
			//arrange
			var author = new BookCreatorUser
			{
				Id = "AuthorId",
				Nickname = "StoryAuthor",
				UserName = "Author",
			};

			var genre = new StoryType
			{
				Id = 1,
				Name = "fantasy"
			};
			var usermanager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			usermanager.CreateAsync(author).GetAwaiter();
			this.Context.StoryTypes.Add(genre);
			this.Context.SaveChanges();

			var newStory = new StoryInputModel
			{
				Author = author.UserName,
				StoryImage = null,
				CreatedOn = DateTime.Now,
				Genre = "fantasy",
				Summary = "someSummary",
				Title = "NewStoryTitle",
			};

			//act
			var storyService = GetService();
			int result = storyService.CreateStory(newStory).GetAwaiter().GetResult();

			var story = this.Context.FictionStories.First();
			//assert

			result.Should().BePositive().And.Subject.Should().Be(1);
			story.Should().NotBeNull().And.Subject.Should().BeEquivalentTo(new
			{
				Id = 1,
				ImageUrl = GlobalConstants.DefaultNoImage,
				newStory.Title,
				Type = new StoryTypeOutputModel
				{
					Id = 1,
					Type = newStory.Genre
				}
			}, options => options.ExcludingMissingMembers());
		}

		[Test]
		public void FollowedStories_Should_Return_Only_stories_User_Is_Following()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "SomeNick",
				UserName = "Follower"
			};

			var userManager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			userManager.CreateAsync(user).GetAwaiter();
			var storyService = GetService();
			var stories = new[]
			{
					new BookCreatorStory
					{
						Title = "One",
						Id = 1,
						CreatedOn = DateTime.Now,
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 1,
							Name = "fantasy"
						},
						AuthorId = "11111",
						Followers = new List<UserStory>
						{
							new UserStory
							{
								BookCreatorStoryId = 1,
								BookCreatorUser = user,
							}
						}
					},
					new BookCreatorStory
					{
						Title = "Two",
						Id = 2,
						CreatedOn = DateTime.Now.AddMonths(1),
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 2,
							Name = "Horror"
						},
						AuthorId = "22222",
						Followers = new List<UserStory>
						{
							new UserStory
							{
								BookCreatorStoryId = 2,
								BookCreatorUser = user,
							}
						}
					},
					new BookCreatorStory
					{
						Title = "three",
						Id = 3,
						CreatedOn = DateTime.Now.AddDays(2),
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 3,
							Name = "Science Fiction"
						},
						AuthorId = "2222223333"
					},
				};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act
			string name = user.UserName;
			var result = storyService.FollowedStories(name);

			//assert
			int count = stories.Length - 1;
			string username = user.UserName;
			result.Should().NotBeEmpty().And.HaveCount(count)
				.And.Subject.As<IEnumerable<StoryOutputModel>>().SelectMany(x => x.Followers.Select(u => u.Username))
				.Should().OnlyContain(x => x == username);
		}

		[Test]
		public void FollowedStoriesByGenre_Should_Return_Only_stories_User_Is_FollowingAndGenre_Selected()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "SomeNick",
				UserName = "Follower"
			};

			var userManager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			userManager.CreateAsync(user).GetAwaiter();
			var storyService = GetService();
			var stories = new[]
			{
					new BookCreatorStory
					{
						Title = "One",
						Id = 1,
						CreatedOn = DateTime.Now,
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 1,
							Name = "fantasy"
						},
						AuthorId = "11111",
						Followers = new List<UserStory>
						{
							new UserStory
							{
								BookCreatorStoryId = 1,
								BookCreatorUser = user,
							}
						}
					},
					new BookCreatorStory
					{
						Title = "Two",
						Id = 2,
						CreatedOn = DateTime.Now.AddMonths(1),
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 2,
							Name = "horror"
						},
						AuthorId = "22222",
						Followers = new List<UserStory>
						{
							new UserStory
							{
								BookCreatorStoryId = 2,
								BookCreatorUser = user,
							}
						}
					},
					new BookCreatorStory
					{
						Title = "three",
						Id = 3,
						CreatedOn = DateTime.Now.AddDays(2),
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 3,
							Name = "Science Fiction"
						},
						AuthorId = "2222223333"
					},
				};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act
			string name = user.UserName;
			string genre = "horror";
			var result = storyService.FollowedStoriesByGenre(name, genre);

			//assert

			string username = user.UserName;
			result.Should().ContainSingle()
				.And.Subject.As<IEnumerable<StoryOutputModel>>().SelectMany(x => x.Followers.Select(u => u.Username))
				.Should().OnlyContain(x => x == username);
		}

		[Test]
		public void FollowedStoriesByGenre_Should_Return_Only_stories_User_Is_Following_And_All_Genres()
		{
			//arrange
			var user = new BookCreatorUser
			{
				Nickname = "SomeNick",
				UserName = "Follower"
			};

			var userManager = this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();
			userManager.CreateAsync(user).GetAwaiter();
			var storyService = GetService();
			var stories = new[]
			{
					new BookCreatorStory
					{
						Title = "One",
						Id = 1,
						CreatedOn = DateTime.Now,
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 1,
							Name = "fantasy"
						},
						AuthorId = "11111",
						Followers = new List<UserStory>
						{
							new UserStory
							{
								BookCreatorStoryId = 1,
								BookCreatorUser = user,
							}
						}
					},
					new BookCreatorStory
					{
						Title = "Two",
						Id = 2,
						CreatedOn = DateTime.Now.AddMonths(1),
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 2,
							Name = "horror"
						},
						AuthorId = "22222",
						Followers = new List<UserStory>
						{
							new UserStory
							{
								BookCreatorStoryId = 2,
								BookCreatorUser = user,
							}
						}
					},
					new BookCreatorStory
					{
						Title = "three",
						Id = 3,
						CreatedOn = DateTime.Now.AddDays(2),
						Summary = null,
						ImageUrl = GlobalConstants.DefaultNoImage,
						Type = new StoryType
						{
							Id = 3,
							Name = "Science Fiction"
						},
						AuthorId = "2222223333"
					},
				};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act
			string name = user.UserName;
			string genre = GlobalConstants.ReturnAllStories;
			var result = storyService.FollowedStoriesByGenre(name, genre);

			//assert
			int count = stories.Length - 1;
			string username = user.UserName;
			result.Should().NotBeEmpty().And.HaveCount(count)
				.And.Subject.As<IEnumerable<StoryOutputModel>>().SelectMany(x => x.Followers.Select(u => u.Username))
				.Should().OnlyContain(x => x == username);
		}

		private IStoryService GetService()
		{
			return this.Provider.GetRequiredService<IStoryService>();
		}
	}
}