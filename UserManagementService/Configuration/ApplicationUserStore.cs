using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Models.Account;

namespace UserManagementService.Configuration
{


    public class ApplicationUserStore : IUserStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser> ,
        IUserAuthenticationTokenStore<ApplicationUser>
    { 
        private readonly ApplicationDbContext _dbContext;
        public ApplicationUserStore(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _dbContext.Users!.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser == null)
                {
                    user.UserId = Guid.NewGuid().ToString();
                    await _dbContext.Users!.AddAsync(user);
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

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var existingUser = await _dbContext.Users!.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
            {
                _dbContext.Users.Remove(existingUser!);
                await _dbContext.SaveChangesAsync();
                return await Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return IdentityResult.Failed(new IdentityError { Code = "400", Description = "The Email has already been taken." });
            }
        }

        public void Dispose()
        {

        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users!.FirstOrDefaultAsync(u => u.UserId.Equals(userId));
            return await Task.FromResult(user);
        }

        public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users!.FirstOrDefaultAsync(u => u.Email == normalizedUserName);
            return await Task.FromResult(user);
        }

        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return Task.FromResult(user.NormalizedUserName);
        }

        public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var existing_user = await _dbContext.Users!.FirstOrDefaultAsync(u => u.Email == user.Email);
            return await Task.FromResult(existing_user?.Id.ToString() ?? null);
        }

        public async Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return await Task.FromResult(user.Email);
        }

        public async Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            user.NormalizedUserName = user.UserName?.ToUpper();
            await Task.CompletedTask;
            //await UpdateAsync(user, cancellationToken);
        }

        public async Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            if (!string.IsNullOrEmpty(userName))
            {
                user.UserName = userName;
                await Task.CompletedTask;
                //await UpdateAsync(user, cancellationToken);
            }
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {

            try
            {
                _dbContext.Users!.Update(user);
                await _dbContext.SaveChangesAsync();
                return await Task.FromResult(IdentityResult.Success);
            }
            catch (DbUpdateException)
            {
                // Handle any database-related exceptions if necessary
                return IdentityResult.Failed(new IdentityError { Code = "ErrorSavingUser", Description = "Error saving user to the database." });
            }
        }

        public async Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            user.Email = email;
            user.NormalizedEmail = email!.ToUpper();
            await Task.CompletedTask;
            //await UpdateAsync(user, cancellationToken);
            //return await Task.FromResult(user);
        }

        public async Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return await Task.FromResult(user!.Email!);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public async Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            user.EmailConfirmed = true;
            await Task.CompletedTask;
            //await UpdateAsync(user, cancellationToken);
            //return Task.FromResult(user);
        }

        public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users!.FirstOrDefaultAsync(u => u.Email == normalizedEmail);
            return await Task.FromResult(user);
        }

        public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return Task.FromResult(user.NormalizedEmail);
        }

        public async Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = user.Email!.ToUpper();
            await Task.CompletedTask;

            //await UpdateAsync(user, cancellationToken);
            //return Task.FromResult(user);
        }

        public async Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            await UpdateAsync(user, cancellationToken);
        }

        public async Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return await Task.FromResult(user.PasswordHash);

            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //var PasswordHash = appUser == null ? "" : appUser.PasswordHash;
            //return await Task.FromResult(PasswordHash);

        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public async Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return await Task.FromResult(user.LockoutEnd);
            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //return appUser?.LockoutEnd;
        }

        public async Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            user.LockoutEnd = lockoutEnd;
            user.LockoutEnabled = lockoutEnd != null;
            await Task.CompletedTask;

            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //if (appUser != null)
            //{
            //    appUser.LockoutEnd = lockoutEnd;
            //    appUser.LockoutEnabled = lockoutEnd != null;
            //    await UpdateAsync(appUser, cancellationToken);
            //}

        }

        public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            user.AccessFailedCount++;
            return await Task.FromResult(user.AccessFailedCount);

            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //if (appUser != null)
            //{
            //    appUser.AccessFailedCount++;
            //    await UpdateAsync(appUser, cancellationToken);
            //    return appUser.AccessFailedCount;
            //}
            //return 0; // Return 0 if user is not found
        }

        public async Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            //user.AccessFailedCount = 0;
            //user.LockoutEnabled = false;
            //user.LockoutEnd = null;
            await Task.CompletedTask;
            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //if (appUser != null)
            //{
            //    appUser.AccessFailedCount = 0;
            //    appUser.LockoutEnabled = false;
            //    appUser.LockoutEnd = null;
            //    await UpdateAsync(appUser, cancellationToken);
            //}
        }

        public async Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return await Task.FromResult(user.AccessFailedCount);

            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //return appUser?.AccessFailedCount ?? 0;
        }

        public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            return await Task.FromResult(user.LockoutEnabled);

            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //return appUser?.LockoutEnabled ?? false;
        }

        public async Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }
            user.LockoutEnabled = enabled;
            await Task.CompletedTask;
            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //if (appUser != null)
            //{
            //    appUser.LockoutEnabled = enabled;
            //    await UpdateAsync(appUser, cancellationToken);
            //}
        }

        public async Task<bool> IsLockedOutAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("User is null.");
            }

            return await Task.FromResult(user.LockoutEnabled);

            //var appUser = await FindByEmailAsync(user.Email!, cancellationToken);
            //return appUser?.LockoutEnabled ?? false;
        }

        public async Task SetTokenAsync(ApplicationUser user, string loginProvider, string name, string? value, CancellationToken cancellationToken)
        {
            // Retrieve the user from the user store
            var existingUser = await FindByEmailAsync(user.Email!, cancellationToken);
            if (existingUser == null)
            {
                throw new ApplicationException("User not found.");
            }
            var token = new IdentityToken()
            {
                UserId = existingUser.UserId,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
            await _dbContext.Tokens.AddAsync(token);
            var result = await UpdateAsync(existingUser, cancellationToken);
            if (!result.Succeeded)
            {
                throw new ApplicationException("Failed to update user.");
            }
        }

        public async Task RemoveTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var existingUser = await FindByEmailAsync(user.Email!, cancellationToken);
            if (existingUser == null)
            {
                throw new ApplicationException("User not found.");
            }
            var token = await _dbContext.Tokens.FirstOrDefaultAsync(u => u.Name == name && u.LoginProvider == loginProvider && u.UserId == user.UserId);
            if (token != null)
            {
                _dbContext.Tokens.Remove(token!);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<string?> GetTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            var existingUser = await FindByEmailAsync(user.Email!, cancellationToken);
            if (existingUser == null)
            {
                throw new ApplicationException("User not found.");
            }
            var token = await _dbContext.Tokens.FirstOrDefaultAsync(u => u.Name == name && u.LoginProvider == loginProvider && u.UserId == user.UserId);
            return token?.Value ?? null;
        }

    }
}



