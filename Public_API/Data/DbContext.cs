using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Public_API.Models.Account;
using Public_API.Models.Arts;

public class ApplicationDbContext : DbContext
{
    public DbSet<ApplicationUser>? Users { get; set; }
    public DbSet<Art>? Arts { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    // dotnet ef migrations add InitialCreate
    // dotnet ef database update

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Rename the default Identity table names to your desired names (optional)
        modelBuilder.Entity<Art>(b =>
        {
            b.HasKey(art => art.ArtId);
            b.ToTable("Arts"); // Change "AspNetUsers"
        });

        // Rename the default Identity table names to your desired names (optional)
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.HasKey(u => u.Id);
            b.ToTable("Users"); // Change "AspNetUsers"

            // A concurrency token for use with the optimistic concurrency checking
            b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            // Limit the size of columns to use efficient database types
            b.Property(u => u.UserName).HasMaxLength(256);
            b.Property(u => u.NormalizedUserName).HasMaxLength(256);
            b.Property(u => u.Email).HasMaxLength(256);
            b.Property(u => u.NormalizedEmail).HasMaxLength(256);
            // The relationships between User and other entity types
            // Note that these relationships are configured with no navigation properties

            //// Each User can have many UserClaims
            //b.HasMany<TUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

            //// Each User can have many UserLogins
            //b.HasMany<TUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

            //// Each User can have many UserTokens
            //b.HasMany<TUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

            //// Each User can have many entries in the UserRole join table
            //b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        });
        /*
        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("AspNetRoles"); // Change "AspNetRoles" to your desired name
        });

        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("AspNetUserRoles"); // Change "AspNetUserRoles" to your desired name
        });

        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("AspNetUserClaims"); // Change "AspNetUserClaims" to your desired name
        });

        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("AspNetUserLogins"); // Change "AspNetUserLogins" to your desired name
        });

        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims"); // Change "AspNetRoleClaims" to your desired name
        });

        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("AspNetUserTokens"); // Change "AspNetUserTokens" to your desired name
        });
        */
    }
       
}