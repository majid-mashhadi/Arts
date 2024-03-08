using System.Net;
using M2Store.Common.ServiceDiscovery;
using M2Store.Contract.Dto;
using M2Store.Contract.Dto.AccountService;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using UserManagementService.Models.Account;
using UserManagementService.Repositories;

namespace UserManagementService.Services
{
    public class EmailService : IEmailRepository
    {
        private readonly IBus bus;
        private readonly int Timeout;
        private readonly MessageQueueDiscovery messageQueueDiscovery;
        public EmailService(
            IBus bus,
            MessageQueueDiscovery messageQueueDiscovery
        )
        {
            this.bus = bus;
            Timeout = 10;
            this.messageQueueDiscovery = messageQueueDiscovery;
        }

        public async Task SendEmailConfirmationEmailAsync(ApplicationUser user, string Token)
        {
            var message = new EmailConfirmationDto(
                (Enum.GetName(typeof(DtoTypes), DtoTypes.EmailConfirmation)!).ToLower(),
                new EmailConfirmationDtoData(user.FirstName!, user.LastName!, user.Email!, Token)
            );
            var endpoint = await bus.GetSendEndpoint(new Uri(messageQueueDiscovery.GetQueueName(MessagingQeueuEnum.EmailConformation)));
            using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                await endpoint.Send(message, cancellationTokenSource.Token);
            }
        }

        public async Task SendForgotPasswordEmailAsync(ApplicationUser user, string Token)
        {
            try
            {
                var message = new ForgotPasswordDto(
                    (Enum.GetName(typeof(DtoTypes), DtoTypes.ForgotPassword)!).ToLower(),
                    new ForgotPasswordDtoData(user.FirstName!, user.LastName!, user.Email!, Token)
                );

                var endpoint = await bus.GetSendEndpoint(new Uri(messageQueueDiscovery.GetQueueName(MessagingQeueuEnum.ForgotPassword)));

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await endpoint.Send(message, cancellationTokenSource.Token); // Set the timeout here
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task SendLockoutNotificationAsync(ApplicationUser user)
        {
            try
            {
                var message = new AccountLockoutDto(
                    (Enum.GetName(typeof(DtoTypes), DtoTypes.AccountLockout)!).ToLower(),
                    new AccountLockoutDtoData(user.FirstName!, user.LastName!, user.Email!)
                );
                var endpoint = await bus.GetSendEndpoint(new Uri(messageQueueDiscovery.GetQueueName(MessagingQeueuEnum.LockoutNotification)));

                using (var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    await endpoint.Send(message, cancellationTokenSource.Token);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

