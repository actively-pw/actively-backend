using MyFitBook.Models.Enums;

namespace MyFitBook.Models.DTOs
{
	public class GetActivityDto
	{
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public string Sport { get; set; }
		public string Start { get; set; }
		public Coordinates StartCoordinates { get; set; }
		public Stats Stats { get; set; }
		public string RouteUrl { get; set; }
		public string LightStaticMapUrl { get; set; }
		public string DarkStaticMapUrl { get; set; }
		public GetActivityDto(Activity activity, StaticMap staticMapType)
		{
			Id = activity.Id;
			Title = activity.Title;
			Sport = SportConverter.SportToString(activity.Sport);
			Start = activity.Start.ToUniversalTime().ToString("o");
			StartCoordinates = new Coordinates(activity.StartLatitude, activity.StartLongitude);
			Stats = new Stats(activity.TotalTime, activity.Distance, activity.AverageSpeed);
			RouteUrl = "https://actively.blob.core.windows.net/geojson-routes/" + Id.ToString() + ".geojson";
			string containerLight, containerDark;
			switch (staticMapType)
			{
				case StaticMap.Web:
					containerLight = "static-maps-web-light";
					containerDark = "static-maps-web-dark";
					break;
				case StaticMap.Mobile:
					containerLight = "static-maps-mobile-light";
					containerDark = "static-maps-mobile-dark";
					break;
				default:
					throw new ArgumentException("Invalid static map type");
			}
			LightStaticMapUrl = "https://actively.blob.core.windows.net/" + containerLight + "/" + Id.ToString() + ".png";
			DarkStaticMapUrl = "https://actively.blob.core.windows.net/" + containerDark + "/" + Id.ToString() + ".png";
		}
	}
	public class Coordinates
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public Coordinates(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}
	}
}
