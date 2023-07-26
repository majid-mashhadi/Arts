using System;
using System.ComponentModel.DataAnnotations;
namespace Public_API.Models.Account
{
	public class SigninRequest
	{
        [Required(ErrorMessage = "Email is required.")]
		[EmailAddress]
		public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string? Password { get; set; }
	}
}

