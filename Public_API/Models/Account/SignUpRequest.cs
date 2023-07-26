using System;
using System.ComponentModel.DataAnnotations;

namespace Public_API.Models.Account
{
	public class SignUpRequest
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }


        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string ?LastName { get; set; }

    }
}

