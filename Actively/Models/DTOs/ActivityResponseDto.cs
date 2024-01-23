using MyFitBook.Models;
using MyFitBook.Models.Enums;

namespace MyFitBook.Models.DTOs
{
	public class ActivityResponseDto
	{
		public Guid Id { get; set; }
		public string? Title { get; set; }
		public Sport Sport { get; set; }
		public DateTime Start { get; set; }
		public double StartLatitude { get; set; }
		public double StartLongitude { get; set; }
		public long TotalTime { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h
		public double MaxSpeed { get; set; }
		public int SumOfAscent { get; set; }
		public int SumOfDescent { get; set; }
		public ActivityResponseDto(Activity activity)
		{
			Id = activity.Id;
			Title = activity.Title;
			Sport = activity.Sport;
			Start = activity.Start;
			StartLatitude = activity.StartLatitude;
			StartLongitude = activity.StartLongitude;
			TotalTime = activity.TotalTime;
			Distance = activity.Distance;
			AverageSpeed = activity.AverageSpeed;
			MaxSpeed = activity.MaxSpeed;
			SumOfAscent = activity.SumOfAscent;
			SumOfDescent = activity.SumOfDescent;
		}
	}
}
