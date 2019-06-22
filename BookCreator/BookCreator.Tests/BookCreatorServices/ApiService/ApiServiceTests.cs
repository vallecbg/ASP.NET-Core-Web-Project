namespace BookCreator.Tests.BookCreatorServices.ApiService
{
	using Base;
	using Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
	using NUnit.Framework;
	using Services.Utilities;
	using Services.Interfaces;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.Extensions.DependencyInjection;
	using ViewModels.OutputModels.ApiOutputModels;

	[TestFixture]
	public class ApiServiceTests : BaseServiceFake
	{
		public IApiService ApiService => this.Provider.GetRequiredService<IApiService>();

		public UserManager<BookCreatorUser> UserManager =>
			this.Provider.GetRequiredService<UserManager<BookCreatorUser>>();

		[Test]
		public void Authors_Should_Return_Only_Users_With_Stories()
		{
			//arrange

			var user = new BookCreatorUser
			{
				UserName = "user",
				Nickname = "userOne"
			};

			var author = new BookCreatorUser
			{
				UserName = "Author",
				Nickname = "AuthorNick"
			};

			this.UserManager.CreateAsync(user).GetAwaiter();
			this.UserManager.CreateAsync(author).GetAwaiter();
			var stories = new BookCreatorStory[]
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
						Name = "Fantasy"
					},
					AuthorId = author.Id
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
					AuthorId = author.Id
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
					AuthorId = "3333"
				},
			};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act

			var result = this.ApiService.Authors();

			//assert

			result.Should().NotBeEmpty().And.HaveCount(1)
				.And.Subject.As<IEnumerable<ApiUserOutputModel>>().First()
				.Username.Should().Be(author.UserName);

			result.First()
				 .Stories.Should().NotBeEmpty()
				 .And.HaveCount(2).And.BeOfType<List<ApiBookCreatorStoryOutputModel>>();
		}

		[Test]
		public void Stories_Should_Return_All_Stories_From_Db()
		{
			//arrange
			var user = new BookCreatorUser
			{
				UserName = "user",
				Nickname = "userOne"
			};

			var author = new BookCreatorUser
			{
				UserName = "Author",
				Nickname = "AuthorNick"
			};

			this.UserManager.CreateAsync(user).GetAwaiter();
			this.UserManager.CreateAsync(author).GetAwaiter();
			var stories = new BookCreatorStory[]
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
						Name = "Fantasy"
					},
					AuthorId = author.Id
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
					AuthorId = author.Id
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
					AuthorId = user.Id
				},
			};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();
			//act

			var result = this.ApiService.Stories();

			//assert

			int count = stories.Length;
			result.Should().NotBeEmpty()
				.And.HaveCount(count)
				.And.AllBeOfType<ApiBookCreatorStoryOutputModel>();
		}

		[Test]
		public void StoriesByGenre_Should_Return_Only_Stories_with_Genre()
		{
			//arrange
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
					AuthorId = "11111"
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
					AuthorId = "22222"
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
			string genre = "fantasy";
			var result = this.ApiService.StoriesByGenre(genre);

			//assert
			result.Should().ContainSingle()
				.And.Subject.First().Genre.Should().Be(genre);
		}

		[Test]
		public void StoriesByGenre_Should_Throw_Exception_With_NoExisting_Genre()
		{
			//arrange
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
					AuthorId = "11111"
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
					AuthorId = "22222"
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
			string genre = "someGenre";
			Func<IEnumerable<ApiBookCreatorStoryOutputModel>> act = () => this.ApiService.StoriesByGenre(genre);

			//assert
			string message = string.Join(GlobalConstants.NoSuchGenre, genre);
			act.Should().Throw<ArgumentException>().WithMessage(message);
		}

		[Test]
		public void TopStories_Should_Return_Stories_Ordered_By_Rating()
		{
			//arrange
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
					Ratings = new List<BookCreatorRating>
					{
						new BookCreatorRating
						{
							StoryRating = new StoryRating
							{
								Rating = 1
							}
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
					Ratings = new List<BookCreatorRating>
					{
						new BookCreatorRating
						{
							StoryRating = new StoryRating
							{
								Rating = 10
							}
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
					AuthorId = "2222223333",
					Ratings = new List<BookCreatorRating>
					{
						new BookCreatorRating
						{
							StoryRating = new StoryRating
							{
								Rating = 5
							}
						}
					}
				},
			};

			this.Context.FictionStories.AddRange(stories);
			this.Context.SaveChanges();

			//act
			var result = this.ApiService.TopStories();

			//assert
			int count = stories.Length;
			double rating = 10;
			result.Should().NotBeEmpty()
				.And.HaveCount(count)
				.And.Subject.As<IEnumerable<ApiBookCreatorStoryOutputModel>>()
				.First().Rating.Should().Be(rating);
		}
	}
}