using MyFitBook.Services.PasswordHasher.Interfaces;
using System.Security.Cryptography;

namespace MyFitBook.Services.PasswordHasher
{
	/// <summary>
	/// Helper class for hashing passwords.
	/// Source: <see href="https://youtu.be/vspPrnZgSAc?si=xizX0wN8qSahBBSO"></see>
	/// </summary>
	public class PasswordHasher : IPasswordHasher
	{
		private const int _saltSize = 128 / 8;
		private const int _keySize = 256 / 8;
		private const int _iterations = 10000;
		private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
		private static char _delimiter = ';';

		/// <summary>
		/// Hashes provided password
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		public string Hash(string password)
		{
			var salt = RandomNumberGenerator.GetBytes(_saltSize);
			var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, _hashAlgorithmName, _keySize);

			return string.Join(_delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
		}

		/// <summary>
		/// Verifies if hashed password matches provided password
		/// </summary>
		/// <param name="passwordHash"></param>
		/// <param name="inputPassword"></param>
		/// <returns></returns>
		public bool Verify(string passwordHash, string inputPassword)
		{
			var elements = passwordHash.Split(_delimiter);
			var salt = Convert.FromBase64String(elements[0]);
			var hash = Convert.FromBase64String(elements[1]);

			var hashedInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, _iterations, _hashAlgorithmName, _keySize);

			return CryptographicOperations.FixedTimeEquals(hashedInput, hash);
		}
	}
}
