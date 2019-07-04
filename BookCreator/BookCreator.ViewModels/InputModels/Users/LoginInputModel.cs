using System.ComponentModel.DataAnnotations;
using BookCreator.ViewModels.Utilities;

namespace BookCreator.ViewModels.InputModels.Users
{
    public class LoginInputModel
    {
        [Required]
        [StringLength(ViewModelsConstants.UserModelNicknameMaxLength, MinimumLength = ViewModelsConstants.UserModelNicknameMinLength)]
        [RegularExpression(ViewModelsConstants.RegexForValidationUsername, ErrorMessage = ViewModelsConstants.ErrorMessageUsernameRegisterModel)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}