namespace MyFitBook.Services.PasswordHasher.Interfaces
{
	/// <summary>
	/// Interface for classes that hash and verify hashed text
	/// </summary>
	public interface IPasswordHasher
	{
		string Hash(string password);
		bool Verify(string passwordHash, string inputPassword);

	}
}
