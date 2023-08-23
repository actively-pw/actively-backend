using Actively.Models.Enums;

namespace Actively.Models.DTOs
{
	public class AddActivityDto
	{
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public Sport Sport { get; set; }
		public Stats Stats { get; set; }
		public RouteSlice[] Route { get; set; }
	}
	public class Stats
	{
		public int Duration { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h

		public Stats(int duration, double distance, double averageSpeed)
		{
			Duration = duration;
			Distance = distance;
			AverageSpeed = averageSpeed;
		}
	}

	public class RouteSlice
	{
		public DateTime Start { get; set; }
		public Location[] Locations { get; set; }
	}

	public class Location
	{
		public DateTime TimeStamp { get; set; }
		public double Latitude{ get; set; }
		public double Longitude { get; set; }
	}
}
