using Azure.Core;
using Microsoft.AspNetCore.Identity;

namespace AuthWith2Fa.Validation
{
    public class CustomPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
    {
        public async Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string? password)
        {
            var username = await manager.GetUserNameAsync(user);
            if (string.Equals(username, password, StringComparison.OrdinalIgnoreCase))
            {
                return IdentityResult.Failed( new IdentityError { Code = "SameUserPass", Description = "Username and Password cant be the same" });
            }
            if (password.ToLower().Contains("password"))
            {
                return IdentityResult.Failed(new IdentityError { Code = "PasswordContainsPassword", Description = "Password Contains Password" });
            }
            return IdentityResult.Success;                     
        }
    }
}
