namespace BookCreatorApp.Extensions
{
	using System.Linq;
	using System.Threading.Tasks;
	using BookCreator.Models;
	using BookCreator.Services.Utilities;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;

	public class SeedRolesMiddleware
	{
		private readonly RequestDelegate _next;

		public SeedRolesMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context,
			UserManager<BookCreatorUser> userManager
			, RoleManager<IdentityRole> roleManager)
		{
			if (!roleManager.Roles.Any())
			{
				await SeedRoles(userManager, roleManager);
			}

			// Call the next delegate/middleware in the pipeline
			await _next(context);
		}

		private async Task SeedRoles(
			UserManager<BookCreatorUser> userManager,
			RoleManager<IdentityRole> roleManager)
		{
			await roleManager.CreateAsync(new IdentityRole
			{
				Name = GlobalConstants.Admin
			});
			await roleManager.CreateAsync(new IdentityRole
			{
				Name = GlobalConstants.Moderator
			});

            ////TODO: Check if needed
			//await roleManager.CreateAsync(new IdentityRole
			//{
			//	Name = GlobalConstants.PaidUser
			//});

			await roleManager.CreateAsync(new IdentityRole
			{
				Name = GlobalConstants.DefaultRole
			});

			var user = new BookCreatorUser
			{
				UserName = "admintest",
				Email = "admin@admin.com",
				Name = "The Admin"
			};

			var normalUser = new BookCreatorUser
			{
				UserName = "usertest",
				Email = "user@user.com",
				Name = "The User"
			};

			string normalUserPass = "user";
			string adminPass = "admin";

			await userManager.CreateAsync(user, adminPass);
			await userManager.CreateAsync(normalUser, normalUserPass);

			await userManager.AddToRoleAsync(user, GlobalConstants.Admin);
			await userManager.AddToRoleAsync(normalUser, GlobalConstants.DefaultRole);
		}
	}
}