using Actively.Models.Enums;

namespace Actively.Models.DTOs
{
    public class GetActivityWithsStatisticsDto
    {
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public Sport Sport { get; set; }
		public string Start { get; set; }
		public DetailedStats Stats { get; set; }
		public string RouteUrl { get; set; }
		public string StaticMapUrl { get; set; }
		public GetActivityWithsStatisticsDto(Activity activity, StaticMap staticMapType)
		{
			Id = activity.Id;
			Title = activity.Title;
			Sport = activity.Sport;
			Start = activity.Start.ToUniversalTime().ToString("o");
			Stats = new DetailedStats(activity.TotalTime, activity.Distance, activity.AverageSpeed,
				activity.MaxSpeed, activity.SumOfAscent, activity.SumOfDescent);
			RouteUrl = "https://actively.blob.core.windows.net/geojson-routes/" + Id.ToString() + ".geojson";
			string container;
			switch (staticMapType)
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
			TotalTime  = totalTime;
			Distance = distance;
			AverageSpeed = averageSpeed;
			MaxSpeed = maxSpeed;
			SumOfAscent = sumOfAscent;
			SumOfDescent = sumOfDescent;
        }
    }
}
