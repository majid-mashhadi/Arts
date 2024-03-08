using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace UserManagementService.Models.Account
{
	public class SigninRequest
	{
		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		public string? Password { get; set; }

		public string? ReturnUrl { get; set; }

		public IList<AuthenticationScheme>? ExternalLogins { get; set; }
	}

	public class ExternalLogin
		{
			public string? Name { get; set; }
			public string? DisplayName { get; set; }
		}

    public class ExternalLoginRequest
    {
        public string? ReturnUrl { get; set; }
        public string? Provider { get; set; }
    }
}

