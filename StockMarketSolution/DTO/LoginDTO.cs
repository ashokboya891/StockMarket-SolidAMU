using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StockMarketSolution.DTO
{
    public class LoginDTO
    {

        [EmailAddress]
        [Required(ErrorMessage = "Please enter email address ")]
        public string Email { set; get; }

        

        [Required(ErrorMessage = "Password cannot be empty")]
        [DataType(DataType.Password)]
        public string Password { set; get; }
    }
}
