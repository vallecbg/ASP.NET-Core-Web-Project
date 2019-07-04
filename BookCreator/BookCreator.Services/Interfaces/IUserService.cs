﻿using BookCreator.ViewModels.InputModels.Users;

namespace BookCreator.Services.Interfaces
{
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Identity;
	using ViewModels.InputModels;
	using ViewModels.OutputModels;
	using ViewModels.OutputModels.Users;

	public interface IUserService
	{
		SignInResult LogUser(LoginInputModel loginModel);

		Task<SignInResult> RegisterUser(RegisterInputModel registerModel);

		IEnumerable<BlockedUserOutputModel> BlockedUsers(string userId);

		void Logout();

		UserOutputViewModel GetUser(string nickName);

		Task BlockUser(string currentUser, string name);

		void UnblockUser(string userId, string id);

        string GetName(string id);

        HomeLoggedModel GetHomeViewDetails();

        //TODO: Need to place it in Message Service to check is it possible to send message
        bool IsBlocked(string user1Name, string user2Name);
    }
}