namespace BookCreator.ViewModels.InputModels
{
	using System.ComponentModel.DataAnnotations;
	using Utilities;

	public class RegisterInputModel
	{
		[Required]
		[StringLength(ViewModelsConstants.UserModelNicknameMaxLength, MinimumLength = ViewModelsConstants.UserModelNicknameMinLength)]
		[RegularExpression(ViewModelsConstants.RegexForValidationNicknameOrUsername, ErrorMessage = ViewModelsConstants.ErrorMessageInRegisterModel)]
		public string Nickname { get; set; }

		[Required]
		[RegularExpression(ViewModelsConstants.RegexForValidationNicknameOrUsername, ErrorMessage = ViewModelsConstants.ErrorMessageInRegisterModel)]
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