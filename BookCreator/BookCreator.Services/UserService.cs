using BookCreator.ViewModels.OutputModels.Books;

namespace BookCreator.Services
{
	using Data;
	using Models;
	using System;
	using Utilities;
	using Interfaces;
	using AutoMapper;
	using System.Linq;
	using System.Threading.Tasks;
	using ViewModels.InputModels;
	using ViewModels.OutputModels;
	using System.Collections.Generic;
	using Microsoft.AspNetCore.Identity;
	using ViewModels.OutputModels.Users;
	using Microsoft.EntityFrameworkCore;
	using AutoMapper.QueryableExtensions;

	public class UserService : BaseService, IUserService
	{
		public UserService(SignInManager<BookCreatorUser> signInManager, UserManager<BookCreatorUser> userManager,
			BookCreatorContext context,
			IMapper mapper)
			: base(userManager, context, mapper)
		{
			this.SignInManager = signInManager;
		}

		protected SignInManager<BookCreatorUser> SignInManager { get; }

		public SignInResult LogUser(LoginInputModel loginModel)
		{
			var user = this.Context.Users.FirstOrDefault(x => x.UserName == loginModel.Username);

			if (user == null)
			{
				return SignInResult.Failed;
			}

			var password = loginModel.Password;
			var result = this.SignInManager.PasswordSignInAsync(user, password, true, false).Result;

			return result;
		}

		public async Task<SignInResult> RegisterUser(RegisterInputModel registerModel)
		{
			bool uniqueNickname = this.Context.Users.All(x => x.Name != registerModel.Name);
			bool uniqueUsername = this.Context.Users.All(x => x.UserName != registerModel.Username);

			if (!uniqueNickname || !uniqueUsername)
			{
				return SignInResult.Failed;
			}

			var user = Mapper.Map<BookCreatorUser>(registerModel);

			await this.UserManager.CreateAsync(user);
			await this.UserManager.AddPasswordAsync(user, registerModel.Password);
			await this.UserManager.AddToRoleAsync(user, GlobalConstants.DefaultRole);
			var result = await this.SignInManager.PasswordSignInAsync(user, registerModel.Password, true, false);

			return result;
		}

		public IEnumerable<BlockedUserOutputModel> BlockedUsers(string userId)
		{
			var blockedUsers = this.Context.BlockedUsers
				.Where(x => x.BookCreatorUserId == userId).Select(x => x.BlockedBookCreatorUser).ProjectTo<BlockedUserOutputModel>(Mapper.ConfigurationProvider).ToArray();

			return blockedUsers;
		}

		public async void Logout()
		{
			await this.SignInManager.SignOutAsync();
		}

		public UserOutputViewModel GetUser(string username)
		{
			var user = this.Context.Users
				.FirstOrDefault(x => x.UserName == username);

			var result = Mapper.Map<UserOutputViewModel>(user);
			result.Role = this.UserManager.GetRolesAsync(user).Result.FirstOrDefault() ?? GlobalConstants.DefaultRole;

			return result;
		}

		public async Task BlockUser(string currentUser, string name)
		{
			var blockingUser = await this.UserManager.FindByNameAsync(currentUser);
			var userTobeBlocked = await this.UserManager.FindByNameAsync(name);

			var Blocked = new BlockedUser
			{
				BookCreatorUser = blockingUser,
				BlockedBookCreatorUser = userTobeBlocked
			};

			bool alreadyBlocked = this.Context.BlockedUsers.Any(x =>
				x.BlockedBookCreatorUser == userTobeBlocked && x.BookCreatorUser == blockingUser);

			if (alreadyBlocked)
			{
				throw new InvalidOperationException(string.Format(GlobalConstants.AlreadyExistsInDb,
					typeof(BlockedUser).Name));
			}

			this.Context.BlockedUsers.Add(Blocked);
			this.Context.SaveChanges();
		}

		public void UnblockUser(string userId, string id)
		{
			var blocked =
				this.Context.BlockedUsers.FirstOrDefault(x =>
					x.BookCreatorUserId == userId && x.BlockedUserId == id);

			if (blocked == null)
			{
				throw new InvalidOperationException(GlobalConstants.NoRecordInDb);
			}

			this.Context.Remove(blocked);
			this.Context.SaveChanges();
		}

        public string GetName(string id)
        {
            var name = this.Context.Users
                .Find(id)
                .Name;

            return name;
        }

        public HomeLoggedModel GetHomeViewDetails()
        {
            var model = new HomeLoggedModel()
            {
                LatestBooks = this.Context.Books
                    .Include(x => x.Chapters)
                    .Include(x => x.Author)
                    .OrderByDescending(x => x.CreatedOn)
                    .Take(2)
                    .ProjectTo<BookHomeOutputModel>(Mapper.ConfigurationProvider)
                    .ToList()
            };

            return model;
        }

        public bool IsBlocked(string user1Name, string user2Name)
        {
            var user1 = this.UserManager.FindByNameAsync(user1Name).GetAwaiter().GetResult();
            var user2 = this.UserManager.FindByNameAsync(user2Name).GetAwaiter().GetResult();

            var notBlocked =
                this.Context.BlockedUsers.Any(x => x.BlockedUserId == user2.Id && x.BookCreatorUserId == user1.Id);

            if (!notBlocked)
            {
                return true;
            }

            return false;
        }
    }
}