using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models.Account
{
	public class IdentityToken
	{
		[Key]
		public Guid Id { get; set; }
		public string? UserId { get; set; }
		public string? LoginProvider { get; set; }
		public string? Name { get; set; }
		public string? Value { get; set; }
	}
}

