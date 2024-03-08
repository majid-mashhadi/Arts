using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models.Account
{
	public class EmailConfirmationRequest
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Token { get; set; }
	}
}

