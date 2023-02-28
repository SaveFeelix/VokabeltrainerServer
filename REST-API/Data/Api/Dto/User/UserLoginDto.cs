#nullable disable
namespace REST_API.Data.Api.Dto.User;

public class UserLoginDto
{
    public UserLoginDto()
    {
    }

    public UserLoginDto(string userName, string password)
    {
        UserName = userName;
        Password = password;
    }

    public string UserName { get; set; }
    public string Password { get; set; }
}