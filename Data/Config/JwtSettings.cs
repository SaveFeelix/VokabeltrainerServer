using System.Text;

namespace REST_API.Data.Config;

public class JwtSettings
{
    public JwtSettings(string token, long expireTime)
    {
        Token = token;
        ExpireTime = expireTime;
    }

    public string Token { get; }
    public byte[] TokenBytes => Encoding.UTF8.GetBytes(Token);
    public long ExpireTime { get; }
}