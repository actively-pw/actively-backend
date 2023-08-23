using Actively.Models.DTOs;
using Actively.Models.Enums;

namespace Actively.Models
{
	public class Activity
	{
        public Guid Id { get; set; }
        //public User User { get; set; }
        public string? Title { get; set; }
        public Sport Sport { get; set; }
        public DateTime Start { get; set; }
		public int TotalTime { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h
		private Activity()
        {
            Id = Guid.NewGuid();
        }
        public Activity(AddActivityDto addActivityDto)
        {
            Id = addActivityDto.Id;
            Title = addActivityDto.Title;
            Sport= addActivityDto.Sport;
            Start = addActivityDto.Route[0].Start;
            TotalTime = addActivityDto.Stats.Duration;
            Distance = addActivityDto.Stats.Distance;
            AverageSpeed = addActivityDto.Stats.AverageSpeed;
        }

    }

}
