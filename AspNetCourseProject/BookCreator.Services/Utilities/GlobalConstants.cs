﻿namespace BookCreator.Services.Utilities
{
	public class GlobalConstants
	{
		public const string Error = "Error";

		public const string ModelError = "LoginError";

		public const string LoginError = "Nickname or password don't match!";

		public const string NicknameOrUsernameNotUnique = "Choose new nickname/username,these {0}/{1} is already in use!";

		public const string NullName = "Name can't be empty";

		public const string Anonymous = "anonymous";

		public const string Success = "Success";

		public const string Failed = "Failed the task";

		public const string UserLackRights = "User don't have rights to make this action";

		public const string NoRecordInDb = "No such record!";

		public const string PaidUser = "paidUser";

		public const string Admin = "admin";

		public const string Moderator = "moderator";

		public const string DefaultRole = "user";

		public const string DbConstName = "dataFromDb";

		public const string ErrorOnDeleteUser = "User was not deleted.Something went wrong!";

		public const string UsernameHolder = "username";

		public const string RedirectAfterAction = "redirectAfterAction";

        public const string UserId = "userId";

		public const string Id = "id";

        public const string AlreadyExistsInDb = "{0} already exists";

		public class RouteConstants
		{
			public const string UserProfileRoute = "/Users/Profile/{username}";

			public const string UserBlockRoute = "/Users/BlockUser/{username}";

			public const string ErrorPageRoute = "/Home/Error";
		}
	}
}