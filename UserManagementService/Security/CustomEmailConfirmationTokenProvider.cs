﻿using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace UserManagementService.Security
{
	public class CustomEmailConfirmationTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
		public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
			IOptions<CustomEmailConfirmationTokenProviderOptions> options, ILogger<DataProtectorTokenProvider<TUser>> logger)
			: base(dataProtectionProvider, options, logger)
		{
		}
	}
}

