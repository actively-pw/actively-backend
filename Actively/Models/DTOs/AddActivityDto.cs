using Actively.Models.Enums;

namespace Actively.Models.DTOs
{
	public class AddActivityDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = "";
		public ActivityType Type { get; set; }
		public DateTime Start { get; set; }
		public Stats Stats { get; set; }
		public CoordsWithTimestamps[] Coordinates { get; set; }
	}
	public class Stats
	{
		public int TotalTime { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h

		public Stats(int totalTime, double distance, double averageSpeed)
		{
			TotalTime = totalTime;
			Distance = distance;
			AverageSpeed = averageSpeed;
		}
	}

	public class CoordsWithTimestamps
	{
		public double X { get; set; }
		public double Y { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
