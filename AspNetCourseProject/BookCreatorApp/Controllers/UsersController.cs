﻿namespace BookCreatorApp.Controllers
{
	using System.Security.Claims;
	using System.Threading.Tasks;
	using BookCreator.Services.Interfaces;
	using BookCreator.Services.Utilities;
	using BookCreator.ViewModels.InputModels;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

	[Authorize]
	public class UsersController : Controller
	{
		public UsersController(IUserService userService)
		{
			UserService = userService;
		}

		protected IUserService UserService { get; }

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			if (this.User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return this.View();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Login(LoginInputModel loginModel)
		{
			if (!ModelState.IsValid)
			{
				return this.View(loginModel);
			}

			var result = this.UserService.LogUser(loginModel);

			if (result != SignInResult.Success)
			{
				this.ViewData[GlobalConstants.ModelError] = GlobalConstants.LoginError;
				return this.View(loginModel);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register()
		{
			if (this.User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return this.View();
		}

		[HttpPost]
		[AllowAnonymous]
		public IActionResult Register(RegisterInputModel registerModel)
		{
			if (!ModelState.IsValid)
			{
				return this.View(registerModel);
			}
			var result = this.UserService.RegisterUser(registerModel).Result;
			if (result != SignInResult.Success)
			{
				this.ViewData[GlobalConstants.ModelError] = string.Format(GlobalConstants.NicknameOrUsernameNotUnique, registerModel.Nickname, registerModel.Username);
				return this.View(registerModel);
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Logout()
		{
			this.UserService.Logout();

			return RedirectToAction("Index", "Home");
		}


		[HttpGet]
		[Route(GlobalConstants.RouteConstants.UserBlockRoute)]
		public async Task<IActionResult> BlockUser(string username)
		{
			var currentUser = this.User.Identity.Name;

			await this.UserService.BlockUser(currentUser, username);

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult BlockedUsers()
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var model = this.UserService.BlockedUsers(userId);

			return this.View(model);
		}

		[HttpGet]
		public IActionResult UnblockUser(string id)
		{
			string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

			this.UserService.UnblockUser(userId, id);

			return RedirectToAction("BlockedUsers", "Users");
		}
	}
}