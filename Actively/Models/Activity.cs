using Actively.Models.DTOs;
using Actively.Models.Enums;
using Actively.Services.StatisticsCalculator;

namespace Actively.Models
{
	public class Activity
	{
        public Guid Id { get; set; }
        public User User { get; set; }
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
		public Activity()
        {
            Id = Guid.NewGuid();
        }
        public Activity(AddActivityDto addActivityDto, ActivityStatistics statistics, User user)
        {
            Id = addActivityDto.Id;
            User = user;
            Title = addActivityDto.Title;
            Sport= addActivityDto.Sport;
            Start = addActivityDto.Route[0].Start;
            StartLatitude = addActivityDto.Route[0].Locations[0].Latitude;
			StartLongitude = addActivityDto.Route[0].Locations[0].Longitude;
			TotalTime = statistics.Duration;
            Distance = statistics.Distance;
            AverageSpeed = statistics.AvgSpeed;
            MaxSpeed = statistics.MaxSpeed;
            SumOfAscent = statistics.SumOfAscent;
            SumOfDescent = statistics.SumOfDescent;
        }

    }

}
