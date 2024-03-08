using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Configuration
{
	public static class DbContextExtension
	{
		public static void SeedDB(this ModelBuilder modelBuilder)
		{
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "Admin",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole
                {
                    Id = "AmazonSeller",
                    Name = "AmazonSeller",
                    NormalizedName = "AMAZONSELLER",
                },
                new IdentityRole
                {
                    Id = "ShippingAdmin",
                    Name = "ShippingAdmin",
                    NormalizedName = "SHIPPPINGADMIN",
                }
            );
        }
	}
}

