using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Models.Account
{
    public class SignUpRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public  string? Password { get; set; }

        //[Required]
        //[Display(Name ="Confirm Password")]
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage ="Password and Confirm password do not match")]
        //public string? ConfirmPassword { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

    }
}

