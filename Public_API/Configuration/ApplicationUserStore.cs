using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Public_API.Models.Account;

namespace Public_API.Configuration
{


    public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>
    {
        private readonly ApplicationDbContext _dbContext;
        public ApplicationUserStore( ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser == null)
                {
                    user.Id = Guid.NewGuid().ToString();
                    await _dbContext.Users.AddAsync(user);
                    await _dbContext.SaveChangesAsync();
                    return await Task.FromResult(IdentityResult.Success);
                }
                else
                {
                    return IdentityResult.Failed(new IdentityError { Code = "400", Description = "The Email has already been taken." });
                }
            }
            catch (DbUpdateException)
            {
                // Handle any database-related exceptions if necessary
                return IdentityResult.Failed(new IdentityError { Code = "ErrorSavingUser", Description = "Error saving user to the database." });
            }
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return await Task.FromResult(user);
        }

        public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedUserName);
            return await Task.FromResult(user);
        }

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var x = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            return await Task.FromResult(x == null ? "" : x.Id);
        }

        public async Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user.Email);
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = user.UserName?.ToUpper();
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            this._dbContext.Users.Update(user);
            this._dbContext.SaveChangesAsync();
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                this._dbContext.Users.Update(user);
                await this._dbContext.SaveChangesAsync();
                return await Task.FromResult(IdentityResult.Success);
            }
            catch (DbUpdateException)
            {
                // Handle any database-related exceptions if necessary
                return IdentityResult.Failed(new IdentityError { Code = "ErrorSavingUser", Description = "Error saving user to the database." });
            }
        }

        public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
        {
            user.Email = email;
            user.NormalizedEmail = email!.ToUpper();
            this._dbContext.Users.Update(user);
            this._dbContext.SaveChanges();
            return Task.FromResult(user);
        }

        public  async Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return await Task.FromResult(user!.Email!);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = true;
            this._dbContext.Users.Update(user);
            this._dbContext.SaveChangesAsync();
            return Task.FromResult(user);
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            return await Task.FromResult(user);
        }

        public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = user.Email!.ToUpper();
            return Task.FromResult(user);
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            this._dbContext.Users.Update(user);
            this._dbContext.SaveChangesAsync();
            return Task.FromResult(user);
        }

        public async Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            var hashedPassword = appUser == null ? "": appUser.PasswordHash;
            return await Task.FromResult(hashedPassword);
            
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }
    }
}



