using MyFitBook.Models.Enums;

namespace MyFitBook.Models.DTOs
{
	public class GetActivityWithsStatisticsDto
	{
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public string Sport { get; set; }
		public string Start { get; set; }
		public Coordinates StartCoordinates { get; set; }
		public DetailedStats Stats { get; set; }
		public string RouteUrl { get; set; }
		public string LightStaticMapUrl { get; set; }
		public string DarkStaticMapUrl { get; set; }
		public GetActivityWithsStatisticsDto(Activity activity, StaticMap staticMapType)
		{
			Id = activity.Id;
			Title = activity.Title;
			Sport = SportConverter.SportToString(activity.Sport);
			Start = activity.Start.ToUniversalTime().ToString("o");
			StartCoordinates = new Coordinates(activity.StartLatitude, activity.StartLongitude);
			Stats = new DetailedStats(activity.TotalTime, activity.Distance, activity.AverageSpeed,
				activity.MaxSpeed, activity.SumOfAscent, activity.SumOfDescent);
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

	public class DetailedStats
	{
		public long TotalTime { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h
		public double MaxSpeed { get; set; }
		public int SumOfAscent { get; set; }
		public int SumOfDescent { get; set; }
		public DetailedStats(long totalTime, double distance, double averageSpeed, double maxSpeed, int sumOfAscent, int sumOfDescent)
		{
			TotalTime = totalTime;
			Distance = distance;
			AverageSpeed = averageSpeed;
			MaxSpeed = maxSpeed;
			SumOfAscent = sumOfAscent;
			SumOfDescent = sumOfDescent;
		}
	}
}
