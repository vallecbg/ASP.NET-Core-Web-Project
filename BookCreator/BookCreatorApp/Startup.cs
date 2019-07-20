
namespace BookCreatorApp
{
	using System;
	using AutoMapper;
	using Extensions;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using BookCreator.Data;
	using BookCreator.Models;
	using BookCreator.Services;
	using BookCreator.Services.Interfaces;
	using BookCreator.Services.Utilities;
	using Microsoft.Extensions.Logging;
    using Hubs;



    public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request.
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
			});

            services.AddSignalR();

			services.AddDbContext<BookCreatorContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
		
    //"Server=(LocalDb)\\.;Database=BookCreatorAppDb;Integrated Security=True;Trusted_Connection=True;MultipleActiveResultSets=true"
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

			services.AddScoped<CustomActionFilterAttribute>();
            services.AddScoped<LogExceptionActionFilter>();

            services.AddScoped<IUserService, UserService>();
			services.AddScoped<IBookService, BookService>();
            services.AddScoped<IChapterService, ChapterService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<INotificationService, NotificationService>();

			services.AddAutoMapper(x => x.AddProfile<BookCreatorProfile>());

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Users/Login";
				options.LogoutPath = $"/Users/Logout";
			});

			services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromSeconds(10));
			services.AddAuthentication()
				.Services.ConfigureApplicationCookie(options =>
				{
					options.SlidingExpiration = true;
					options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
				});

			services.AddMvc(opt =>
				{
					opt.Filters.Add<CustomActionFilterAttribute>();
					opt.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
				})
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseSeedRolesMiddleware();
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "areas",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

				routes.MapRoute(
					 name: "default",
					 template: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseCookiePolicy();

            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
        }
	}
}