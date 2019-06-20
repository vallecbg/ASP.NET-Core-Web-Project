namespace BookCreator.ViewModels.InputModels
{
    using Utilities;
    using System.ComponentModel.DataAnnotations;

    public class LoginInputModel
    {
        [Required]
        [StringLength(ViewModelsConstants.UserModelNicknameMaxLength, MinimumLength = ViewModelsConstants.UserModelNicknameMinLength)]
        [RegularExpression(ViewModelsConstants.RegexForValidationNicknameOrUsername)]
        public string Nickname { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}