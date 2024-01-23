namespace MyFitBook.Models.DTOs
{
	public class AddActivityDto
	{
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public string Sport { get; set; }
		public Stats Stats { get; set; }
		public RouteSlice[] Route { get; set; }
		public AddActivityDto()
		{
			Stats = new Stats(0, 0, 0);
			Route = new RouteSlice[1];
			Route[0] = new RouteSlice();
		}
	}
	public class Stats
	{
		public long Duration { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h

		public Stats(long duration, double distance, double averageSpeed)
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
		public RouteSlice()
		{
			Locations = new Location[2];
		}
	}

	public class Location
	{
		public DateTime TimeStamp { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double Altitude { get; set; }
	}
}
