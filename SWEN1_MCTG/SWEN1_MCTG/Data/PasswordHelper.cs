using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Password hashing helper class
/// </summary>
public static class PasswordHelper
{
    /// <summary>
    /// Hashes a password using SHA256
    /// </summary>
    /// <param name="password"> An unhashed password </param>
    /// <returns> SHA256 Hash for the password </returns>
    public static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
