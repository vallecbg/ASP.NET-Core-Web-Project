using System.ComponentModel.DataAnnotations;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.InputModels.Users
{
    public class RegisterInputModel
	{
		[Required]
		[StringLength(ViewModelsConstants.UserModelNicknameMaxLength, MinimumLength = ViewModelsConstants.UserModelNicknameMinLength)]
		[RegularExpression(ViewModelsConstants.RegexForValidationName, ErrorMessage = ViewModelsConstants.ErrorMessageNameRegisterModel)]
		public string Name { get; set; }

		[Required]
		[RegularExpression(ViewModelsConstants.RegexForValidationUsername, ErrorMessage = ViewModelsConstants.ErrorMessageUsernameRegisterModel)]
		public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[Compare(nameof(Password))]
		[Display(Name = ViewModelsConstants.ConfirmPasswordDisplay)]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }

		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
	}
}