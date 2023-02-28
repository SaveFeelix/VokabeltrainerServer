using System.Security.Cryptography;
using System.Text;

namespace REST_API.Data.Db.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public byte[] PasswordSalt { get; set; }
    public byte[] PasswordHash { get; set; }

    public async Task<bool> GeneratePassword(string password)
    {
        try
        {
            var hmacSha512 = new HMACSHA512();
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(password));
            PasswordHash = await hmacSha512.ComputeHashAsync(stream);
            PasswordSalt = hmacSha512.Key;
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<bool> ValidatePassword(string password)
    {
        try
        {
            var hmacSha512 = new HMACSHA512(PasswordSalt);
            await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(password));
            byte[] passwordHash = await hmacSha512.ComputeHashAsync(stream);
            return PasswordHash.SequenceEqual(passwordHash);
        }
        catch
        {
            return false;
        }
    }
}