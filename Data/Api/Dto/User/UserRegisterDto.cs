#nullable disable
namespace REST_API.Data.Api.Dto.User;

public class UserRegisterDto
{
    public UserRegisterDto()
    {
    }

    public UserRegisterDto(string userName, string email, string password)
    {
        UserName = userName;
        Email = email;
        Password = password;
    }

    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}