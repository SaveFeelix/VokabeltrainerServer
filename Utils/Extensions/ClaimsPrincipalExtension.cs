using System.Security.Claims;
using REST_API.Data.Db;
using REST_API.Data.Db.Models;

namespace REST_API.Utils.Extensions;

public static class ClaimsPrincipalExtension
{
    public static async Task<User?> GenerateUserFromJwt(this ClaimsPrincipal principal, DataContext context)
    {
        var claim = principal.Claims.FirstOrDefault(it => it.Type == nameof(User.Id));
        if (claim is null)
            return null;
        var user = await context.Users.FindAsync(int.Parse(claim.Value));
        return user;
    }
}