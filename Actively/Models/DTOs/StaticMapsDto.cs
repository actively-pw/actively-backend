namespace Actively.Models.DTOs
{
	public class StaticMapsDto : IDisposable
	{
		public Stream WebLight { get; set; }
		public Stream MobileLight { get; set; }
		public Stream WebDark { get; set; }
		public Stream MobileDark { get; set; }

		public void Dispose() 
		{
			WebLight?.Dispose();
			MobileLight?.Dispose();
			WebDark?.Dispose();
			MobileDark?.Dispose();
		}

	}
}
