using System.Security.Cryptography;

namespace Actively.Services.PasswordHasher
{
	public class PasswordHasher
	{
		private const int _saltSize = 128 / 8;
		private const int _keySize = 256 / 8;
		private const int _iterations = 10000;
		private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
		private static char _delimiter = ';';

		public string Hash(string password)
		{
			var salt = RandomNumberGenerator.GetBytes(_saltSize);
			var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _keySize);

			return string.Join(_delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
		}
	}
}
