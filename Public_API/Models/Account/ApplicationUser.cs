using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Public_API.Models.Account
{
    public class ApplicationUser: IdentityUser
    {
        //[Key]
        //public string userId { get; set; }

        //[Required]
        //public string Email { get; set; }

        //[Required]
        //public string Password { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public DateTime Created { get; set; }
        public bool IsActive { get; set; }

    }
}

