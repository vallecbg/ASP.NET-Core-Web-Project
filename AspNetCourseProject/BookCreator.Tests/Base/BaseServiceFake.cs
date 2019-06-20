namespace BookCreator.Tests.Base
{
	using Data;
	using System;
	using Models;
	using Services;
	using AutoMapper;
	using NUnit.Framework;
	using Services.Utilities;
	using Services.Interfaces;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.Extensions.DependencyInjection;

	[TestFixture]
	public class BaseServiceFake
	{
		protected IServiceProvider Provider { get; set; }

		protected BookCreatorContext Context { get; set; }

		[SetUp]
		public void SetUp()
		{
			Mapper.Reset();
			Mapper.Initialize(x => { x.AddProfile<FanfictionProfile>(); });
			var services = SetServices();
			this.Provider = services.BuildServiceProvider();
			this.Context = this.Provider.GetRequiredService<BookCreatorContext>();
			SetScoppedServiceProvider();
		}

		[TearDown]
		public void TearDown()
		{
			this.Context.Database.EnsureDeleted();
		}

		//<summary>
		// why i need this? because Di make problems for dependencies and instead of mocking!
		// as i had to write a lot more code and i already had everything  working
		//  i found this work around SigninManager need httpContext but context need services
		// so basic now with every request sign in manager will get context with the DI it needs
		// took me 2 hours to figure it out,without mocking god bless stack overflow :D
		// </summary>
		private void SetScoppedServiceProvider()
		{
			var httpContext = this.Provider.GetService<IHttpContextAccessor>();
			httpContext.HttpContext.RequestServices = this.Provider.CreateScope().ServiceProvider;
		}

		private ServiceCollection SetServices()
		{
			var services = new ServiceCollection();

			services.AddDbContext<BookCreatorContext>(
				opt => opt.UseInMemoryDatabase(Guid.NewGuid()
					.ToString()));

			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IAdminService, AdminService>();
			services.AddScoped<IStoryService, StoryService>();
			services.AddScoped<IChapterService, ChapterService>();
			services.AddScoped<ICommentService, CommentService>();
			services.AddScoped<IMessageService, MessageService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			services.AddScoped<IApiService, ApiService>();

			services.AddIdentity<BookCreatorUser, IdentityRole>(opt =>
				{
					opt.Password.RequireDigit = false;
					opt.Password.RequireLowercase = false;
					opt.Password.RequireNonAlphanumeric = false;
					opt.Password.RequireUppercase = false;
					opt.Password.RequiredLength = 3;
					opt.Password.RequiredUniqueChars = 0;
				})
				.AddEntityFrameworkStores<BookCreatorContext>()
				.AddDefaultTokenProviders();

			services.AddAutoMapper();

			var context = new DefaultHttpContext();

			services.AddSingleton<IHttpContextAccessor>(
				new HttpContextAccessor()
				{
					HttpContext = context,
				});
			return services;
		}
	}
}