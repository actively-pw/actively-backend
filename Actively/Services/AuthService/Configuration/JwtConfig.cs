namespace MyFitBook.Services.AuthService.Configuration
{
	/// <summary>
	/// Contains basic information about JWTs
	/// </summary>
	public class JwtConfig
	{
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public string Key { get; set; }
		public int LifetimeInSeconds { get; set; }
		public int RefreshTokenLifetimeInDays { get; set; }
	}
}
