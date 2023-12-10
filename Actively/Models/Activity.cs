using Actively.Models.DTOs;
using Actively.Models.Enums;
using Actively.Services.StatisticsCalculator;

namespace Actively.Models
{
	public class Activity
	{
        public Guid Id { get; set; }
        //public User User { get; set; }
        public string? Title { get; set; }
        public Sport Sport { get; set; }
        public DateTime Start { get; set; }
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
        public Activity(AddActivityDto addActivityDto, ActivityStatistics statistics)
        {
            Id = addActivityDto.Id;
            Title = addActivityDto.Title;
            Sport= addActivityDto.Sport;
            Start = addActivityDto.Route[0].Start;
            TotalTime = statistics.Duration;
            Distance = statistics.Distance;
            AverageSpeed = statistics.AvgSpeed;
            MaxSpeed = statistics.MaxSpeed;
            SumOfAscent = statistics.SumOfAscent;
            SumOfDescent = statistics.SumOfDescent;
        }

    }

}
