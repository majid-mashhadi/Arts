using System;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Public_API.Configuration
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                // Define your API resources if needed
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
        {
            // Define your clients (applications) here
            new Client
            {
                ClientId = "YOUR_CLIENT_ID",
                ClientSecrets = { new Secret("YOUR_CLIENT_SECRET".Sha256()) },
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RedirectUris = { "https://your-app.com/callback" },
                PostLogoutRedirectUris = { "https://your-app.com/signout-callback" },
                AllowedScopes =
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "YOUR_API_RESOURCE_SCOPE"
                },
                AllowOfflineAccess = true
            }
        };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                // Define your test users if needed
            };
        }
    }

}

