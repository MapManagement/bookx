using System.Security.Cryptography;
using System.Text;

namespace Bookx.Helpers;

public static class CryptographyHelper
{
    #region Fields

    private const string EnvPasswordPepper = "PASSWORD_PEPPER";
    private const int _keySize = 64;
    private const int _iterations = 500_000;
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA3_512;
    private static readonly string _passwordPepper;

    #endregion

    #region Constructor

    static CryptographyHelper()
    {
        _passwordPepper = Environment.GetEnvironmentVariable(EnvPasswordPepper);
    }

    #endregion

    #region Methods

    public static (string hash, string salt) CreatePasswordHash(string password)
    {
        string passwordSalt = GeneratePasswordSalt();
        string passwordHash = HashSaltedPepperedText(password, passwordSalt);

        return (passwordHash, password);
    }

    public static bool VerfiyPasswordHash(string password, string salt, string dbHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
            return false;

        string computedPasswordHash = HashSaltedPepperedText(password, salt);

        bool areHashesEqual = CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(computedPasswordHash),
                Convert.FromHexString(dbHash)
            );

        return areHashesEqual;
    }

    private static string HashSaltedPepperedText(string text, string salt)
    {
        string pepperedText = new StringBuilder(text).Append(_passwordPepper).ToString();

        byte[] computedHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(text),
                Convert.FromHexString(salt),
                _iterations,
                _hashAlgorithmName,
                _keySize
            );

        return Convert.ToHexString(computedHash);
    }

    private static string GeneratePasswordSalt()
    {
        return Convert.ToHexString(RandomNumberGenerator.GetBytes(_keySize));
    }

    #endregion
}
