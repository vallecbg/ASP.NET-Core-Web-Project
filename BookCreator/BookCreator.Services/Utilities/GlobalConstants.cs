using System.Runtime.InteropServices.WindowsRuntime;

namespace BookCreator.Services.Utilities
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

		public const string ErrorOnDeleteUser = "User was not deleted. Something went wrong!";

		public const string UsernameHolder = "username";

		public const string RedirectAfterAction = "redirectAfterAction";

        public const string UserId = "userId";

		public const string Id = "id";

        public const string AlreadyExistsInDb = "{0} already exists";

        public const string ReturnAllBooks = "All";

        public const string UnknownUser = "Unknown User";

        public const string NoImageAvailableUrl =
            "https://res.cloudinary.com/vallec/image/upload/v1561301682/No_image_available_zvvugj.png";

        public static readonly string[] ImageExtensions = { "png", "jpg", "jpeg" };

        public static string WrongFileType = $"The image type should be: {string.Join(", ", ImageExtensions)}";

        public const string BookNotFound = "No Book with such Id was found.";

        public const string BookId = "bookId";

        public const string ChapterLength = "ChapterLength";

        public const string ChapterId = "chapterId";

        public const string UserHasNoRights = "User need role Admin or Moderator for this action!";

        public const string NotAuthor = "To do this action you need to be author of this book!";

        public const string NotValidChapterStoryConnection = "This book with id: {0} or chapter with id: {1} don't have a connection";

        public const string CommentsLength = "Comment should not be empty";

        public const string AlreadyRated = "You have already rated this book";

        public const string EmptyMessage = "The message cannot be empty!";

        public const string AlreadyFollowed = "You have already followed this book!";

        public const string NotFollowing = "You aren't following this book!";
        
        public const string ErrorOnDeleteGenre = "An error happened during the deletion of this genre!";

        public const string TooShortAnnouncement = "Your announcement has to be at least 5 characters long";

        public class RouteConstants
		{
			public const string UserProfileRoute = "/Users/Profile/{username}";

			public const string UserBlockRoute = "/Users/BlockUser/{username}";

			public const string ErrorPageRoute = "/Home/Error";

            public const string UserBooksRoute = "/Books/UserBooks/{username}";

            public const string AddChapterRoute = "/Chapters/AddChapter/{bookId}";

            public const string Administration = "Administration";

        }

        public class CloudinaryConfig
        {
            public const string CloudinaryCloudName = "vallec";

            public const string CloudinaryApiKey = "148382891263925";

            public const string CloudinaryApiSecret = "GDijvH1mRWflHJa0J6oerHATqqI";

            public const string CloudinaryEnvironmentVariable = "cloudinary://148382891263925:GDijvH1mRWflHJa0J6oerHATqqI@vallec/";
        }
    }
}