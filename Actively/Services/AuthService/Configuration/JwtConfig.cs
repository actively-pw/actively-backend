namespace Actively.Services.AuthService.Configuration
{
	public class JwtConfig
	{
		public string Issuer { get; set; }
		public string Audience { get; set; }
		public string Key { get; set; }
		public int LifetimeInSeconds { get; set; }
	}
}
