using System.Security.Cryptography;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bookx.Helpers;

public static class CryptographyHelper
{
    #region Fields

    private const string EnvPasswordPepper = "PASSWORD_PEPPER";
    private const string EnvJwtSecurityKey = "JWT_SECURITY_KEY";
    private const int _keySize = 64;
    private const int _iterations = 500_000;
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA3_512;
    private static readonly string _passwordPepper;
    internal static readonly SymmetricSecurityKey JwtSecurityKey;

    #endregion

    #region Constructor

    static CryptographyHelper()
    {
        _passwordPepper = Environment.GetEnvironmentVariable(EnvPasswordPepper);
        string securityKey = Environment.GetEnvironmentVariable(EnvJwtSecurityKey);
        JwtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
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

    public static string GenerateJwt(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username)
        };

        var credentials = new SigningCredentials(JwtSecurityKey, SecurityAlgorithms.Sha512);
        // TODO: read domain form env var?
        var jwt = new JwtSecurityToken(
                issuer: "YOUR_DOMAIN",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
        );

        string serializedToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        return serializedToken;
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
