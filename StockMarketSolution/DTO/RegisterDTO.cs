
using Microsoft.AspNetCore.Mvc;
using StockMarketSolution.Enum;
using System.ComponentModel.DataAnnotations;

namespace StockMarketSolution.DTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage ="Name is mandtory filed please enter ")]
        public string PersonName { set; get; }

        [Required]
        public string Token { set; get; }

        [Remote(action:"IsEmailAddressExisted",controller:"Account",ErrorMessage ="Email already in use try newone")]
        [EmailAddress]
        [Required(ErrorMessage ="Please enter email address ")]
        public string Email { set; get; }

        [RegularExpression("^[0-9]*$",ErrorMessage ="Phone number should only conatin numerics")]
        [Required(ErrorMessage ="Phone number cannot be empty")]
        public string PhoneNumber { set; get; }

        [Required(ErrorMessage ="Password cannot be empty")]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "ConfirmPassword cannot be empty")]
        [Compare("Password",ErrorMessage ="Password and confirmPassword should match")]
        public string ConfirmPassword { set; get; }

        [Required(ErrorMessage = "you have to choose one")]
        public UserTypeOptions UserType { set; get; }=UserTypeOptions.User;
    }
}
