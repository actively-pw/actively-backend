using Actively.Models.Enums;

namespace Actively.Models.DTOs
{
	public class GetActivityDto
	{
		public Guid Id { get; set;}
		public string? Title { get; set;}
		public Sport Sport { get; set;}
		public string Start { get; set;}
		public Coordinates StartCoordinates { get; set;}
		public Stats Stats { get; set;}
		public string RouteUrl { get; set;}
		public string StaticMapUrl { get; set;}
		public GetActivityDto(Activity activity, StaticMap staticMapType)
		{
			Id = activity.Id;
			Title = activity.Title;
			Sport = activity.Sport;
			Start = activity.Start.ToUniversalTime().ToString("o");
			StartCoordinates = new Coordinates(activity.StartLatitude, activity.StartLongitude);
			Stats = new Stats(activity.TotalTime, activity.Distance, activity.AverageSpeed);
			RouteUrl = "https://actively.blob.core.windows.net/geojson-routes/" + Id.ToString() + ".geojson";
			string container;
			switch(staticMapType)
			{
				case StaticMap.WebLight:
					container = "static-maps-web-light";
					break;
				case StaticMap.MobileLight:
					container = "static-maps-mobile-light";
					break;
				default:
					throw new ArgumentException("Invalid static map type");
			}
			StaticMapUrl = "https://actively.blob.core.windows.net/" + container + "/" + Id.ToString() + ".png";
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
