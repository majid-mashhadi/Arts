using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace UserManagementService.Models.Account
{
    public class CustomUserRole : IdentityUserRole<Guid> { }
    public class CustomUserClaim : IdentityUserClaim<Guid> { }
    public class CustomUserLogin : IdentityUserLogin<Guid> { }
    public class CustomUserToken : IdentityUserToken<Guid> { }
    public class CustomRoleClaim : IdentityRoleClaim<Guid> { };
    public class CustomRole : IdentityRole<Guid>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    //public class CustomUserStore : UserStore<ApplicationUser, CustomRole, Guid, CustomUserLogin, CustomUserRole, CustomUserClaim>
    //    public class CustomUserStore : UserStore<ApplicationUser> //UserStore<ApplicationUser,CustomRole, ApplicationDbContext, Guid, CustomUserClaim,CustomUserLogin, CustomUserToken, CustomRoleClaim>
    //{
    //    public CustomUserStore(ApplicationDbContext context)
    //        : base(context)
    //    {
    //    }
    //}

    //public class CustomRoleStore : RoleStore<CustomRole, Guid, CustomUserRole>
    //{
    //    public CustomRoleStore(ApplicationDbContext context)
    //        : base(context)
    //    {
    //    }
    //}
}

