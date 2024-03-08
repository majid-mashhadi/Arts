using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace UserManagementService.Models.Account
{
    public class ApplicationUser : IdentityUser
    {
        [Key]
        [JsonPropertyName("UserId")]
        public override string Id { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        public DateTime Created { get; set; }

        public bool IsActive { get; set; }
        [NotMapped]
        public string UserId
        {
            get
            {
                return Id!;
            }
            set
            {
                Id = value;
            }
        }
    }
}

