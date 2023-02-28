using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_API.Data.Api.Dto.User;
using REST_API.Data.Db;
using REST_API.Data.Db.Models;
using REST_API.Utils.Services;

namespace REST_API.Data.Api.Controller;

[ApiController]
[Route("[controller]/[action]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly DataContext _context;
    private readonly JwtService _jwtService;

    public AuthController(JwtService jwtService, DataContext context)
    {
        _jwtService = jwtService;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Register(UserRegisterDto registerDto)
    {
        try
        {
            if (await ValidateEmail(registerDto.Email))
                return Conflict(nameof(registerDto.Email));
            if (await ValidateUserName(registerDto.UserName))
                return Conflict(nameof(registerDto.UserName));
            var user = new User
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };
            await user.GeneratePassword(registerDto.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch
        {
            return Problem();
        }

        return Ok();
    }


    [HttpPost]
    public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
    {
        var token = await _jwtService.RequestToken(loginDto.UserName, loginDto.Password);
        if (string.IsNullOrEmpty(token))
            return Unauthorized();
        return Ok(token);
    }

    private async Task<bool> ValidateEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(it => it.Email.ToLower() == email.ToLower());
        return user is not null;
    }

    private async Task<bool> ValidateUserName(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(it => it.UserName.ToLower() == userName.ToLower());
        return user is not null;
    }
}