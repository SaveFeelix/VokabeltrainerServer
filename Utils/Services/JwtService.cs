using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using REST_API.Data.Config;
using REST_API.Data.Db;

namespace REST_API.Utils.Services;

public class JwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly DataContext _context;

    public JwtService(DataContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    public async Task<string?> RequestToken(string userName, string password)
    {
        var user =
            await _context.Users.FirstOrDefaultAsync(
                it => it.UserName.ToLower() == userName.ToLower());
        if (user is null)
            return null;

        if (!await user.ValidatePassword(password))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.Token);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(nameof(user.Id), user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpireTime),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha512Signature)
        };
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}