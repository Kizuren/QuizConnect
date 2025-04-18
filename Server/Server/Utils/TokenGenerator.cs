using System.Security.Cryptography;

namespace WritingServer.Utils;

public static class TokenGenerator
{
    public static string IdGenerator(int length)
    {
        Random random = new Random();
        string numbers = "0123456789ABCDEF";
        string chars = string.Empty;
        for (int i = 0; i < length; i++)
        {
            chars += numbers[random.Next(numbers.Length)];
        }
        return chars;
    }

    public static string GenerateToken(int length)
    {
        byte[] randomBytes = new byte[length / 2];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        
        string hexString = Convert.ToHexString(randomBytes);
        
        return hexString.Length > length ? hexString.Substring(0, length) : hexString;
    }
}