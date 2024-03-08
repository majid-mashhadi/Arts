using UserManagementService.Models.Account;

namespace UserManagementService.Repositories
{
	public interface IEmailRepository
	{
        Task SendEmailConfirmationEmailAsync(ApplicationUser user, string Token);
        Task SendForgotPasswordEmailAsync(ApplicationUser user, string Token);
        Task SendLockoutNotificationAsync(ApplicationUser user);
    }
}

